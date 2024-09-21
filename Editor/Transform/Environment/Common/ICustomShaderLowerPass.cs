using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// カスタムシェーダーをUniGLTFが認識できるシェーダーへ変換する。
    /// UniGLTFが認識できるシェーダーはカスタムシェーダーとプロパティの互換性があるとは限らないため、カスタムシェーダーについての変換を
    /// 定義する際にはこのインターフェースも実装することが<b>強く</b>推奨される。<br />
    /// UniGLTFが認識できるシェーダーには、v0.125.0時点で以下のものがある。
    /// このリストは非網羅的であり、かつ今後のアップデートにより増減する可能性があることに注意。
    /// <list type="bullet">
    /// <item>Standardシェーダー (<a href="https://github.com/vrm-c/UniVRM/blob/f8120fb22dade99ea8a9e03b77a3bbc2a4c79806/Assets/UniGLTF/Runtime/UniGLTF/IO/MaterialIO/BuiltInRP/Export/BuiltInGltfMaterialExporter.cs#L9">ソース</a>)</item>
    /// <item>Built-in Render PipelineのUnlitシェーダー (<a href="https://github.com/vrm-c/UniVRM/blob/v0.125.0/Assets/UniGLTF/Runtime/UniGLTF/IO/MaterialIO/BuiltInRP/Export/BuiltInGltfMaterialExporter.cs#L11-L14">ソース</a>)</item>
    /// <item>UniUnlit</item>
    /// </list>
    ///
    /// 通常はNormal MapやMetallic Gross Mapを格納できるStandardシェーダーにするのが得策だろう。
    /// </summary>
    internal interface ICustomShaderLowerPass
    {
        [NotPublicAPI]
        public Material LowerInline(Material m);
    }
}
