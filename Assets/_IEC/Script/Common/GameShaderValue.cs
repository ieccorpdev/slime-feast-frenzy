using UnityEngine;

namespace IEC
{
    public static class GameShaderValue
    {
        public static int SHADER_FILL_AMOUNT = Shader.PropertyToID("_FillAmount");
        public static int SHADER_POS = Shader.PropertyToID("_MaskPos");
        public static int SHADER_COLOR = Shader.PropertyToID("_MainColor");

        public static int SHADER_TILING = Shader.PropertyToID("_Tiling");
        public static int SHADER_DIST_TILING = Shader.PropertyToID("_DistTiling");
        public static int SHADER_MOVE_DIRECTIONG = Shader.PropertyToID("_MoveDirection");
        public static int SHADER_WATER_TEX = Shader.PropertyToID("_WaterTex");
        public static int SHADER_DIST_TEX = Shader.PropertyToID("_DistTex");
        public static int SHADER_VERSION = Shader.PropertyToID("_Version");
    }
}