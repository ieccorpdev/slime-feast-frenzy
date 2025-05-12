// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("vNGOBbELwhDrOCv05E/uYimKLpJq3AZuLVUfh/RmeVFMIRK8ZOQgX8tIRkl5y0hDS8tISEmS6mRJimv5sF9EkoGDYQA40EK+D8rRactWc7gmcFFso6Ej9lpRf113RVz0fS0F67wPlbFZoc0KmDroVB0wOu6lK5zObc4zpQSm5s+IGpn40/3FqCqu9tt5y0hreURPQGPPAc++REhISExJSkTAwo17/ni41ZUbpUQJ8P0ZfjXJ1+FzmL3/EjyhiNTB98G2pEc4JcEf0eTzSJjbsoG0I0b/7HFZUqWXkeawjwtxn6EdVYxVAmTjJNZGoLC4UufmITQpJYTtaHrkZWKD+npsCfMzE55ceDw3ujqTfyHE6fNqPZt07B511a9Eo8InhEtKSElI");
        private static int[] order = new int[] { 4,6,6,8,11,13,12,11,10,11,13,11,13,13,14 };
        private static int key = 73;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
