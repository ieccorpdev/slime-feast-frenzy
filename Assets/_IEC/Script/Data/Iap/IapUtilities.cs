using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing;

namespace IEC
{
    public class IapUtilities : _BaseBehaviour
    {
        public static List<string> GetProductsID()
        {
            var productCatalog = ProductCatalog.LoadDefaultCatalog();
            return productCatalog.allProducts.Select(item => item.id).ToList();
        }

        public static List<ProductCatalogPayout> GetProductPayOuts(string id)
        {
            var product = GetProductById(id);
            return product?.Payouts.ToList();
        }

        public static ProductCatalogItem GetProductById(string id)
        {
            var productCatalog = ProductCatalog.LoadDefaultCatalog();
            return  productCatalog.allProducts.FirstOrDefault(x => x.id == id);
        }
        public static List<string> GetProductsIDType(ProductType productType)
        {
            var productCatalog = ProductCatalog.LoadDefaultCatalog();
            return (from item in productCatalog.allProducts where item.type == productType select item.id).ToList();
        }
        
        public static ResourceType ConvertCurrencyType(string subType)
        {
            string subTypeInLowerCase = subType.ToLower();
            if (subTypeInLowerCase.Equals("helperClear"))
            {
                return ResourceType.HelperClear;
            }
            else if (subTypeInLowerCase.Equals("helperUndo"))
            {
                return ResourceType.HelperUndo;
            }
            else if (subTypeInLowerCase.Equals("helperShuffle"))
            {
                return ResourceType.HelperShuffle;
            }
            else if (subTypeInLowerCase.Equals("timeInfiniteLife"))
            {
                return ResourceType.UnlimitedLive;
            }
            else return ResourceType.Gold;
        }

        //public static SubscriptionType ConvertSubscriptionType(string productID)
        //{
        //    if (productID == PRODUCT_VIP_PASS_1) return SubscriptionType.Vip1;
        //    if (productID == PRODUCT_VIP_PASS_2) return SubscriptionType.Vip2;
        //    if (productID == PRODUCT_VIP_PASS_3) return SubscriptionType.Vip3;
        //    return SubscriptionType.None;
        //}
        //public static string ConvertSubscriptionTypeToId(SubscriptionType type)
        //{
        //    if (type == SubscriptionType.Vip1) return PRODUCT_VIP_PASS_1;
        //    if (type == SubscriptionType.Vip2) return PRODUCT_VIP_PASS_2;
        //    if (type == SubscriptionType.Vip3) return PRODUCT_VIP_PASS_3;
        //    return "";
        //}

        public const string PRODUCT_VIP_PASS_1 = "com.screw.saga.quest.nrs.vippass.1";
        public const string PRODUCT_VIP_PASS_2 = "com.screw.saga.quest.nrs.vippass.2";
        public const string PRODUCT_VIP_PASS_3 = "com.screw.saga.quest.nrs.vippass.3";

        public const string PRODUCT_REMOVE_ADS_ONLY = "com.screw.unjam.puzzle.free.ads";
        public const string PRODUCT_REMOVE_ADS_BUNDLE = "com.screw.unjam.puzzle.free.ads.bundle";

    }
}