using ResoniteImportHelper.Allocator;
using UnityEngine;

namespace ResoniteImportHelper.Serialization
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

        internal readonly ResourceAllocator Allocator;

        internal SerializationConfiguration(
            GameObject processingTemporaryObjectRoot,
            GameObject originalMaybePackedObject,
            bool generateIntermediateArtifact,
            ResourceAllocator allocator
        )
        {
            ProcessingTemporaryObjectRoot = processingTemporaryObjectRoot;
            OriginalMaybePackedObject = originalMaybePackedObject;
            GenerateIntermediateArtifact = generateIntermediateArtifact;
            Allocator = allocator;
        }
    }
}
