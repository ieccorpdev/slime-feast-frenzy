using UnityEngine;

namespace IEC
{
    public static class GameShaderValue
    {
        public static int SHADER_FILL_AMOUNT = Shader.PropertyToID("_FillAmount");
        public static int SHADER_POS = Shader.PropertyToID("_GLOBALMaskPosition");
        public static int SHADER_IS_GRAY = Shader.PropertyToID("_HaveGray");
    }
}