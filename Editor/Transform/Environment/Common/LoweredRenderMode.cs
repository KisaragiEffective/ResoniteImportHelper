#nullable enable
using ResoniteImportHelper.Marker;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    [NotPublicAPI]
    public enum LoweredRenderMode
    {
        /// <summary>
        /// 不明。
        /// </summary>
        Unknown,
        /// <summary>
        /// 完全に不透明。アルファチャンネルは無視される。
        /// </summary>
        Opaque,
        /// <summary>
        /// カットアウト。アルファマスクはベースカラーのアルファチャンネルを参照する。
        /// </summary>
        Cutout,
        /// <summary>
        /// 半透明。
        /// </summary>
        Blend,
    }
}