using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.MarketingModule.Core;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.MarketingModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Promotions;
using VirtoCommerce.MarketingModule.Core.Search;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.ExportImport;
using VirtoCommerce.MarketingModule.Data.Handlers;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.MarketingModule.Data.Search;
using VirtoCommerce.MarketingModule.Data.Services;
using VirtoCommerce.MarketingModule.Web.Authorization;
using VirtoCommerce.MarketingModule.Web.ExportImport;
using VirtoCommerce.OrdersModule.Core.Events;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.ExportImport;
using VirtoCommerce.Platform.Core.JsonConverters;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;

namespace VirtoCommerce.MarketingModule.Web
{
    [ExcludeFromCodeCoverage]
    public class Module : IModule, IExportSupport, IImportSupport
    {
        private IApplicationBuilder _appBuilder;
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<MarketingDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString(ModuleInfo.Id) ?? configuration.GetConnectionString("VirtoCommerce"));
            });
            serviceCollection.AddTransient<IMarketingRepository, MarketingRepository>();
            serviceCollection.AddTransient<Func<IMarketingRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IMarketingRepository>());

            #region Services

            serviceCollection.AddTransient<IPromotionService, PromotionService>();
            serviceCollection.AddTransient<ICouponService, CouponService>();
            serviceCollection.AddTransient<IPromotionUsageService, PromotionUsageService>();
            serviceCollection.AddTransient<IMarketingDynamicContentEvaluator, DefaultDynamicContentEvaluator>();
            serviceCollection.AddTransient<IDynamicContentService, DynamicContentService>();
            serviceCollection.AddTransient<IPromotionRewardEvaluator, DefaultPromotionRewardEvaluator>();

            #endregion

            #region Search

            serviceCollection.AddTransient<IContentItemsSearchService, ContentItemsSearchService>();
            serviceCollection.AddTransient<IContentPlacesSearchService, ContentPlacesSearchService>();
            serviceCollection.AddTransient<IContentPublicationsSearchService, ContentPublicationsSearchService>();
            serviceCollection.AddTransient<ICouponSearchService, CouponSearchService>();
            serviceCollection.AddTransient<IFolderSearchService, FolderSearchService>();
            serviceCollection.AddTransient<IPromotionSearchService, PromotionSearchService>();
            serviceCollection.AddTransient<IPromotionUsageSearchService, PromotionUsageSearchService>();

            #endregion

            serviceCollection.AddTransient<CsvCouponImporter>();

            serviceCollection.AddTransient<IMarketingPromoEvaluator>(provider =>
            {
                var settingsManager = provider.GetService<ISettingsManager>();
                var platformMemoryCache = provider.GetService<IPlatformMemoryCache>();
                var promotionService = provider.GetService<IPromotionSearchService>();
                var promotionCombinePolicy = settingsManager.GetValue(ModuleConstants.Settings.General.CombinePolicy.Name, "BestReward");

                if (promotionCombinePolicy.EqualsInvariant("CombineStackable"))
                {
                    var promotionRewardEvaluator = provider.GetService<IPromotionRewardEvaluator>();
                    return new CombineStackablePromotionPolicy(promotionService, promotionRewardEvaluator, platformMemoryCache);
                }

                return new BestRewardPromotionPolicy(promotionService, platformMemoryCache);
            });

            serviceCollection.AddTransient<LogChangesChangedEventHandler>();
            serviceCollection.AddTransient<MarketingExportImport>();
            serviceCollection.AddTransient<CouponUsageRecordHandler>();

            serviceCollection.AddTransient<IAuthorizationHandler, MarketingAuthorizationHandler>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;

            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);

            var permissionsRegistrar = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsRegistrar.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Marketing",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            //Register Permission scopes
            AbstractTypeFactory<PermissionScope>.RegisterType<MarketingStoreSelectedScope>();
            permissionsRegistrar.WithAvailabeScopesForPermissions(new[] { ModuleConstants.Security.Permissions.Read }, new MarketingStoreSelectedScope());

            // Register DynamicPromotion override
            var couponSearchService = appBuilder.ApplicationServices.GetRequiredService<ICouponSearchService>();
            var promotionUsageSearchService = appBuilder.ApplicationServices.GetRequiredService<IPromotionUsageSearchService>();
            AbstractTypeFactory<Promotion>.RegisterType<DynamicPromotion>().WithSetupAction((promotion) =>
            {
                var dynamicPromotion = promotion as DynamicPromotion;
                dynamicPromotion.CouponSearchService = couponSearchService;
                dynamicPromotion.PromotionUsageSearchService = promotionUsageSearchService;
            });

            var eventHandlerRegistrar = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            //Create order observer. record order coupon usage
            eventHandlerRegistrar.RegisterHandler<OrderChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CouponUsageRecordHandler>().Handle(message));

            eventHandlerRegistrar.RegisterHandler<PromotionChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesChangedEventHandler>().Handle(message));
            eventHandlerRegistrar.RegisterHandler<CouponChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesChangedEventHandler>().Handle(message));

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<MarketingDbContext>();
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            var dynamicPropertyRegistrar = appBuilder.ApplicationServices.GetRequiredService<IDynamicPropertyRegistrar>();
            dynamicPropertyRegistrar.RegisterType<DynamicContentItem>();

            var dynamicContentService = appBuilder.ApplicationServices.GetService<IDynamicContentService>();
            foreach (var id in new[] { ModuleConstants.MarketingConstants.ContentPlacesRootFolderId, ModuleConstants.MarketingConstants.CotentItemRootFolderId })
            {
                var folders = dynamicContentService.GetFoldersByIdsAsync(new[] { id }).GetAwaiter().GetResult();
                var rootFolder = folders.FirstOrDefault();
                if (rootFolder == null)
                {
                    rootFolder = new DynamicContentFolder
                    {
                        Id = id,
                        Name = id
                    };
                    dynamicContentService.SaveFoldersAsync(new[] { rootFolder }).GetAwaiter().GetResult();
                }
            }

            //Create standard dynamic properties for dynamic content item
            var dynamicPropertyService = appBuilder.ApplicationServices.GetService<IDynamicPropertyService>();
            var contentItemTypeProperty = new DynamicProperty
            {
                Id = "Marketing_DynamicContentItem_Type_Property",
                IsDictionary = true,
                Name = "Content type",
                ObjectType = typeof(DynamicContentItem).FullName,
                ValueType = DynamicPropertyValueType.ShortText,
                CreatedBy = "Auto",
            };

            dynamicPropertyService.SaveDynamicPropertiesAsync(new[] { contentItemTypeProperty }).GetAwaiter().GetResult();

            PolymorphJsonConverter.RegisterTypeForDiscriminator(typeof(PromotionReward), nameof(PromotionReward.Id));

            //Register the resulting trees expressions into the AbstractFactory<IConditionTree> 
            foreach (var conditionTree in AbstractTypeFactory<PromotionConditionAndRewardTreePrototype>.TryCreateInstance().Traverse<IConditionTree>(x => x.AvailableChildren))
            {
                AbstractTypeFactory<IConditionTree>.RegisterType(conditionTree.GetType());
            }
            foreach (var conditionTree in AbstractTypeFactory<DynamicContentConditionTreePrototype>.TryCreateInstance().Traverse<IConditionTree>(x => x.AvailableChildren))
            {
                AbstractTypeFactory<IConditionTree>.RegisterType(conditionTree.GetType());
            }
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }

        public Task ExportAsync(Stream outStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<MarketingExportImport>().DoExportAsync(outStream, options, progressCallback, cancellationToken);
        }

        public Task ImportAsync(Stream inputStream, ExportImportOptions options, Action<ExportImportProgressInfo> progressCallback,
            ICancellationToken cancellationToken)
        {
            return _appBuilder.ApplicationServices.GetRequiredService<MarketingExportImport>().DoImportAsync(inputStream, options, progressCallback, cancellationToken);
        }
    }
}
