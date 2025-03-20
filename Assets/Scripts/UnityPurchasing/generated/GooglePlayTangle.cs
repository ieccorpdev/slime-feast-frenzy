// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("KfuImCSHSTD4V86vBH5jaQijwIVWPjjK6GErcls2DOvnJL5lh84q9e3qXAdym11obUCKQ30bwBgfo+VQZdF9hYeckCO06u/HrncaTzzljuLU6gFbduOESnjjXBs5y6M5aLrFguha2fro1d7R8l6QXi/V2dnZ3djbWtnX2Oha2dLaWtnZ2HTvXqfLCgKuvjKKB264rYzftEXXtBx8iwdwSdq+vFXc5kqNUR2jIM6v0DjxFHgBVdvcVUJg1RgJQnWd8NWIa1PBuHhd1WZtszRn4fMXAahrGxTd/tt+g9mWpsc6UqaFH9rDhZT3N4D/12k1A04nLo9Z+7hl85roPhLuOJca7SI3uLEN1RbQNgM+e3teYO54VjiiSArlfbjSEbCD+9rb2djZ");
        private static int[] order = new int[] { 7,3,8,5,13,7,7,8,12,12,13,12,13,13,14 };
        private static int key = 216;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
