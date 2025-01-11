#nullable enable
using KisaragiMarine.ResoniteImportHelper.Allocator;
using KisaragiMarine.ResoniteImportHelper.Generic.Collections;
using KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common;
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Serialization
{
    internal class SerializationConfiguration
    {
        /// <summary>
        /// パイプラインパスが変更を加えるために<see cref="OriginalMaybePackedObject"/> をシャローコピーして一時的に確保されたオブジェクト。
        /// コンポーネントが指す参照先までは差し替わっていないため注意。
        /// すべてのパイプラインパスが終了した後に自動的に消滅する。
        /// </summary>
        internal readonly GameObject ProcessingTemporaryObjectRoot;
        /// <summary>
        /// 現在処理を行っているオブジェクトのルート。パイプラインパスはこのオブジェクトのヒエラルキーやコンポーネント、その設定の値などを一切変更してはならない。
        /// 変更した場合、それは実装者のエラーである。
        /// 変更用に確保されたオブジェクトについては <see cref="ProcessingTemporaryObjectRoot"/> を見ること。
        /// </summary>
        internal readonly GameObject OriginalMaybePackedObject;

        /// <summary>
        /// 変換後のデータを参照するために、全ての処理が完了した後かつ、glTFにシリアライズする前にOriginal Prefabとして中間アーティファクトを残す。
        /// </summary>
        internal readonly bool GenerateIntermediateArtifact;

        /// <summary>
        /// シリアライズする前の段階で透明、または半透明として描画されるべきと判断された<see cref="Material"/>を列挙する。
        /// </summary>
        internal readonly MultipleUnorderedDictionary<LoweredRenderMode, Material> MaterialsConsideredToBeTransparent;

        internal readonly ResourceAllocator Allocator;

        internal SerializationConfiguration(
            GameObject processingTemporaryObjectRoot,
            GameObject originalMaybePackedObject,
            bool generateIntermediateArtifact,
            MultipleUnorderedDictionary<LoweredRenderMode, Material> materialsConsideredToBeTransparent,
            ResourceAllocator allocator
        )
        {
            ProcessingTemporaryObjectRoot = processingTemporaryObjectRoot;
            OriginalMaybePackedObject = originalMaybePackedObject;
            GenerateIntermediateArtifact = generateIntermediateArtifact;
            MaterialsConsideredToBeTransparent = materialsConsideredToBeTransparent;
            Allocator = allocator;
        }
    }
}
