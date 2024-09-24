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
    /// <remarks>
    /// 変換先のシェーダーをStandardシェーダーにする場合、かつ変換元シェーダーが半透明またはカットアウトで描画する場合、第一引数を<c>RenderMode</c>にした<see cref="Material.SetOverrideTag(string, string)"/>を忘れないこと。<br />
    /// さもなくばUniGLTFはBlendModeが不透明であるとみなし、結果としてテクスチャのアルファチャンネルの損失を招く (<a href="https://github.com/vrm-c/UniVRM/blob/f8120fb22dade99ea8a9e03b77a3bbc2a4c79806/Assets/UniGLTF/Runtime/UniGLTF/IO/MaterialIO/BuiltInRP/Export/Materials/BuiltInStandardMaterialExporter.cs#L40-L57">ソース</a>)。
    /// </remarks>
    /// </summary>
    internal interface ICustomShaderLowerPass
    {
        /// <summary>
        /// カスタムシェーダーを用いているかもしれないMaterialをUniGLTFが認識できるシェーダーで描画されるMaterialへ変換する。
        /// この実装は、次のように実装されなければならない:
        /// <list type="bullet">
        /// <item>
        /// ある<see cref="Material" /> <c>m</c> に対して、<c>LowerInline(m).GetMaybeConvertedMaterial()</c>が
        /// <c>LowerInline(LowerInline(m).GetMaybeConvertedMaterial()).GetMaybeConvertedMaterial()</c> と等しいこと。
        /// </item>
        /// <item>
        /// 同じ入力を与えられたときに、同じ出力を返すこと。<br />
        /// ある<c>Material a</c>及び<c>Material b</c>が存在し、<c>a == b</c>であるとき、以下の等式も成り立つこと。
        /// <list type="bullet">
        /// <item><c>LowerInline(a).GetMaybeConvertedMaterial() == LowerInline(b).GetMaybeConvertedMaterial()</c></item>
        /// <item><c>LowerInline(a).GetComputedRenderMode() == LowerInline(b).GetComputedRenderMode()</c></item>
        /// </list>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [NotPublicAPI]
        public ISealedLoweredMaterialReference LowerInline(Material m);
    }
}
