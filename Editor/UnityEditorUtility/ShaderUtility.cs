using UnityEditor;
using UnityEngine;

namespace ResoniteImportHelper.UnityEditorUtility
{
    public static class ShaderUtility
    {
        /// <summary>
        /// Standard シェーダーを確実に取得する。
        /// </summary>
        /// <returns></returns>
        public static Shader GetStandardShaderReliably()
        {
            // `Shader.Find("Standard")`はShaderLabで"Standard"というシェーダーが定義されていた場合にシャドウされてしまう。
            // Standard shaderは"Builtin extra resource"扱いなのでAssets下やPackages下には見つからない。
            // see: https://kan-kikuchi.hatenablog.com/entry/GetBuiltinResource
            return AssetDatabase.GetBuiltinExtraResource<Shader>("Standard.shader");
        }
    }
}
