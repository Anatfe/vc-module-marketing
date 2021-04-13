﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using Newtonsoft.Json;

namespace IsSatisfiedByBench
{
    public class LinqVsPlainIsSatisfiedBy
    {
        private const string childrenstring = @"[{""All"":false,""Not"":false,""Id"":""BlockCustomerCondition"",""AvailableChildren"":[],""Children"":[{""Id"":""ConditionIsEveryone"",""AvailableChildren"":[],""Children"":[]}]},{""All"":false,""Not"":false,""Id"":""BlockCatalogCondition"",""AvailableChildren"":[],""Children"":[]},{""All"":false,""Not"":false,""Id"":""BlockCartCondition"",""AvailableChildren"":[],""Children"":[{""SubTotal"":1000.0,""SubTotalSecond"":0.0,""ExcludingCategoryIds"":[],""ExcludingProductIds"":[],""CompareCondition"":""AtLeast"",""Id"":""ConditionCartSubtotalLeast"",""AvailableChildren"":[],""Children"":[]}]},{""Id"":""BlockReward"",""AvailableChildren"":[],""Children"":[{""ProductId"":""3df7597b-0ab8-4a81-a05a-9f8baa2b96b9"",""ProductName"":""Product97"",""NumItem"":2,""Id"":""RewardItemGetFreeNumItemOfProduct"",""AvailableChildren"":[],""Children"":[]},{""Amount"":100.0,""Id"":""RewardCartGetOfAbsSubtotal"",""AvailableChildren"":[],""Children"":[]}]}]";
        private const string contextstring = @"{""RefusedGiftIds"":null,""StoreId"":""TestStore"",""Currency"":""USD"",""CustomerId"":""Anonymous"",""IsRegisteredUser"":false,""IsFirstTimeBuyer"":false,""IsEveryone"":true,""CartTotal"":0.0,""ShipmentMethodCode"":null,""ShipmentMethodOption"":null,""ShipmentMethodPrice"":0.0,""AvailableShipmentMethodCodes"":null,""PaymentMethodCode"":null,""PaymentMethodPrice"":0.0,""AvailablePaymentMethodCodes"":null,""Coupon"":null,""Coupons"":null,""CartPromoEntries"":[],""PromoEntries"":[{""Code"":""SKU-9-16-290"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""11FA7C30-0C27-4C31-99AE-709831046E4B"",""ProductId"":""4B70F12A-25F8-4225-9A50-68C7E6DA25B2"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-12-11-274"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""5E38DC24-3C6C-4429-9097-4E5CE4B24C44"",""ProductId"":""4B711D2B-0125-4637-B73E-1C21D63AE8A1"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-9-5-67"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""7466B7F8-D75F-46DC-BE98-67A4131118AA"",""ProductId"":""4B716012-4548-4894-A277-D530D395A507"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-17-48-88"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""C2975F4B-F70E-4193-AD4C-7D992EDA1329"",""ProductId"":""4B718FD7-DBD7-4F76-B66D-18B312BCF813"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-17-3-195"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""48FA1A51-9F5E-437C-A8A4-57B821B39799"",""ProductId"":""4B72D383-8A0E-4973-B752-CA37EF055C25"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-11-23-40"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""16EAA539-B2A3-4EB1-99DB-093BF88FDD62"",""ProductId"":""4B736149-685C-4619-AF99-91101E61108D"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-17-51-25"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""24C2F72C-E83A-4ECD-88F5-976FFBEF9934"",""ProductId"":""4B73ACF3-E71C-4ED3-AB58-352D4272B585"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-10-27-271"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""E9E6B65A-EA6C-4D4B-9909-35F042D480BC"",""ProductId"":""4B742A12-42DE-4DB5-8B5D-B897D0C5F6C4"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-12-48-76"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""409BDEEA-0430-4434-BDDC-CC7E81B47F77"",""ProductId"":""4B747EA2-14D1-43EB-99EF-EFC23880ABF1"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-10-41-91"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""7461A306-5C6C-429B-B395-F23874B9D85D"",""ProductId"":""4B748206-7D6E-4A93-A3A4-300BD5C8B5A0"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-9-4-299"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""848962A1-5CCB-4F20-A093-300B3D197910"",""ProductId"":""4B7559F8-B3C0-447F-B085-0E937324A97F"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-10-38-55"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""65F477EB-0A1E-415A-81ED-5143A24F3E31"",""ProductId"":""4B75A509-1BC0-4F5B-BFE8-642124A1C3F8"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-5-33-217"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""CCFDFCF0-B432-45A5-B259-1A9034932DEB"",""ProductId"":""4B761603-ED38-430C-8FF8-504142407FF4"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-16-4-40"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""260BB0D7-67B0-4285-81A4-079172FAA522"",""ProductId"":""4B76C306-E2C2-45F0-9465-C898880FB16C"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-6-50-58"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""E34C6E5B-9DFF-4190-9391-8BA1FBFA3226"",""ProductId"":""4B770944-D73E-4BAC-AD72-81A4AD7F67A5"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-1-30-290"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""971957E1-5420-4391-8D9C-66D2ECA8B066"",""ProductId"":""4B77336C-C716-45F3-88F4-938B0C7E4D55"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-15-19-224"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""1A3213A0-094C-4665-9324-BFD4C6ECB55C"",""ProductId"":""4B77E8CB-9FD4-41F4-B1B8-826E482441E4"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-9-27-153"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""E59EAA98-6CFB-4391-AD4E-0A3CAF1309C0"",""ProductId"":""4B77ECC2-4BC2-4697-94D0-ABF9E8920EB2"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-14-29-272"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""7FADC97A-08EB-4BF5-AA96-86DAA005761D"",""ProductId"":""4B785842-7A14-446E-BC32-944B31D297BA"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-1-38-273"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""E7DBACC1-614E-4CA1-A936-C02831655855"",""ProductId"":""4B7874B1-96EB-4FF9-9482-ACB28E5691C1"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-19-38-161"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""526DD10B-E9D0-4282-A044-2D2F18AF7236"",""ProductId"":""4B7881A0-FD9A-4826-BE55-F1FD87381AF8"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-21-5-76"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""6953F6DD-53B9-4135-9FB6-F3EA44A06B6B"",""ProductId"":""4B7890FA-5B30-4B8A-AE2F-DF2E6EDF415E"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-7-41-166"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""1978439D-32C7-4BC5-AC5A-0EB4294530F5"",""ProductId"":""4B78ED66-6D08-4249-BA18-1BE224FF2E67"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-10-27-173"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""E9E6B65A-EA6C-4D4B-9909-35F042D480BC"",""ProductId"":""4B79745D-531E-4CEF-AA6A-43A751A68F59"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-13-44-215"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""3E6034E4-8B2F-47DB-8188-4F0204B0C9B1"",""ProductId"":""4B7976B3-B108-4D96-8612-72A547173FEC"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-9-23-10"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""B49ED63F-30D2-4B9C-8776-A7B4E960B545"",""ProductId"":""4B7A2950-8851-4C39-9F9D-D19456C2825F"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-12-11-254"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""5E38DC24-3C6C-4429-9097-4E5CE4B24C44"",""ProductId"":""498DD6E8-E9B5-452E-A813-B7329BBD7CB6"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-17-45-171"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""79BD9D03-4A41-4222-A584-B48B741BD948"",""ProductId"":""498E01AC-923E-4A0D-8AB8-F637AFBEA9A6"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-10-21-283"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""B3D3D137-CB50-4E53-9A7D-8EB393C820DC"",""ProductId"":""498E2639-189D-425B-9B37-B2EB55408F31"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-11-26-235"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""48732971-F52E-410D-B227-B4233F252A83"",""ProductId"":""498E2EC2-68D9-46C6-A36F-B71C589D3B13"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-10-16-37"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""6D18F17E-3EA7-4F66-97AE-4E2239E6D9BB"",""ProductId"":""498F12F7-41A1-4F79-80F6-7B7E2362C191"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-9-3-52"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""5B54CC7E-A4E9-49D5-B466-C7C6008F222B"",""ProductId"":""498F4014-1386-429F-B4F4-817FC30F496D"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-8-47-145"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""934E489C-213D-40F9-8F95-E6395368ADDD"",""ProductId"":""498F7D51-CB0A-496D-B723-1271612743DA"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-6-36-220"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""5F96966C-B5DB-4A76-8DBC-505B2CBAD057"",""ProductId"":""498F8C45-9392-4CAF-87F5-4475C84BBDCD"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-19-51-175"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""43557048-6A5B-4A5D-946D-6E7FB8274D31"",""ProductId"":""499020AA-CBEA-4C8C-89F5-EE49EE30AF2A"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-16-10-233"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""20971F49-59D3-4936-A0B5-F718EFC541C2"",""ProductId"":""49903247-7139-4275-BC6B-B1592AECFAE9"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-18-32-158"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""5EA9AA31-178E-4CBD-B9DC-DD0245229DDC"",""ProductId"":""499138F0-F633-4C2F-8759-140C58FA3E05"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-17-2-288"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""8E8C1F54-6291-43A1-B2AF-19AE7E2BA747"",""ProductId"":""4991F327-BDFF-4327-B0F8-79FC49C7DBEB"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-15-17-232"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""38EBCABB-5A94-463C-AFBB-57D2BF4086BF"",""ProductId"":""4991FCFF-1587-4901-9D8E-AD8A18CD0B84"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-12-15-265"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""C5C37A77-B4E9-48ED-A2DC-D56F00E3F11D"",""ProductId"":""49922DD5-0F2E-433D-BCD8-37E641D3F06D"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-16-3-227"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""9E4196AA-89E9-4773-86FA-FD7A82F45F89"",""ProductId"":""4992B073-E15F-44FB-927F-EBD22BD7EAD0"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-10-21-91"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""B3D3D137-CB50-4E53-9A7D-8EB393C820DC"",""ProductId"":""499328F2-E95E-4F14-BE21-496ED428D1B2"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-6-21-158"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""03CA3A26-3386-46ED-866D-E66D475097F9"",""ProductId"":""49934CD5-6AC8-4A14-AFC9-DACFFC472C2E"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-6-36-252"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""5F96966C-B5DB-4A76-8DBC-505B2CBAD057"",""ProductId"":""49939F5F-ECB5-4267-B0AA-4E885D1D862E"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-6-39-136"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""C7094A3D-54A9-42A2-ACCB-2C41B0EBDD5B"",""ProductId"":""4994175C-3427-4371-8B74-E1C22A4DBA53"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-21-16-247"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""529DE5CD-C990-4087-8F66-F0D49B84B3CF"",""ProductId"":""499485FE-AEF8-44E1-B08C-2B0A94496D2A"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-12-35-250"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""CA4AA3EC-2CB9-4056-8934-7D57F1D48575"",""ProductId"":""4994959D-CE69-4671-9280-2244E6C67930"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-18-50-172"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""36BE6909-1207-4AFD-8F9D-113AB0E1F493"",""ProductId"":""4994E579-DE48-4B9A-AD62-B52C5114A0F4"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-5-43-277"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""8B77CD0F-5C4E-4BBA-9AEF-CD3022D0D2C1"",""ProductId"":""49950FD0-44C7-4C50-91F2-2C598030F039"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},{""Code"":""SKU-9-7-1"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""0AF2F1DB-6B0C-42A9-AEB1-B2DDE9098AB2"",""ProductId"":""49954D3B-425E-4067-B695-5EA38875B117"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}}],""PromoEntry"":{""Code"":""SKU-9-16-290"",""Quantity"":1,""InStockQuantity"":0,""Price"":100.0,""ListPrice"":100.0,""Discount"":0.0,""CatalogId"":""09d9043d-59cd-4792-9a30-49207ec344a5"",""CategoryId"":""11FA7C30-0C27-4C31-99AE-709831046E4B"",""ProductId"":""4B70F12A-25F8-4225-9A50-68C7E6DA25B2"",""Owner"":null,""Outline"":null,""Variations"":[],""Attributes"":{}},""ContextObject"":null,""GeoCity"":null,""GeoState"":null,""GeoCountry"":null,""GeoContinent"":null,""GeoZipCode"":null,""GeoConnectionType"":null,""GeoTimeZone"":null,""GeoIpRoutingType"":null,""GeoIspSecondLevel"":null,""GeoIspTopLevel"":null,""ShopperAge"":0,""ShopperGender"":null,""Language"":null,""UserGroups"":null,""ShopperSearchedPhraseInStore"":null,""ShopperSearchedPhraseOnInternet"":null,""CurrentUrl"":null,""ReferredUrl"":null}";

        private readonly BlockConditionAndOrLinq blockConditionAndOrLINQ = new BlockConditionAndOrLinq();
        private readonly BlockConditionAndOrPlain blockConditionAndOrPlain = new BlockConditionAndOrPlain();
        private readonly PromotionEvaluationContext context;

        public LinqVsPlainIsSatisfiedBy()
        {
            context = JsonConvert.DeserializeObject<PromotionEvaluationContext>(contextstring);
            blockConditionAndOrLINQ.Children = JsonConvert.DeserializeObject<List<BlockConditionAndOrLinq>>(childrenstring);
            blockConditionAndOrPlain.Children = JsonConvert.DeserializeObject<List<BlockConditionAndOrPlain>>(childrenstring);
        }

        [Benchmark]
        public bool IsSatisfiedByWithLINQ() => blockConditionAndOrLINQ.IsSatisfiedBy(context);

        [Benchmark]
        public bool IsSatisfiedByWithPlain() => blockConditionAndOrPlain.IsSatisfiedBy(context);
    }

    internal static class Program
    {
        static void Main()
        {
            // new LINQvsPlainIsSatisfiedBy().IsSatisfiedByWithLINQ(); // Debug
            BenchmarkRunner.Run<LinqVsPlainIsSatisfiedBy>(); // Test
        }
    }
}