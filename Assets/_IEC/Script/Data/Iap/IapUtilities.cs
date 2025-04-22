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
            if (subTypeInLowerCase.Equals("helperVortex"))
            {
                return ResourceType.Vortex;
            }
            else if (subTypeInLowerCase.Equals("helperCompass"))
            {
                return ResourceType.Compass;
            }
            else if (subTypeInLowerCase.Equals("helperEnlarge"))
            {
                return ResourceType.Enlarge;
            }
            else if (subTypeInLowerCase.Equals("helperTimeStop"))
            {
                return ResourceType.TimeStop;
            }
            else if (subTypeInLowerCase.Equals("timeInfiniteLife"))
            {
                return ResourceType.UnlimitedLive;
            }
            else return ResourceType.Gold;
        }


        public const string PRODUCT_VIP_PASS_1 = "com.screw.saga.quest.nrs.vippass.1";
        public const string PRODUCT_VIP_PASS_2 = "com.screw.saga.quest.nrs.vippass.2";
        public const string PRODUCT_VIP_PASS_3 = "com.screw.saga.quest.nrs.vippass.3";

        public const string PRODUCT_REMOVE_ADS_ONLY = "com.screw.unjam.puzzle.free.ads";
        public const string PRODUCT_REMOVE_ADS_BUNDLE = "com.screw.unjam.puzzle.free.ads.bundle";

    }
}