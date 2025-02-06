#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.UnityEditorUtility
{
    internal static class RendererUtility
    {
        /// <summary>
        /// <see cref="MeshRenderer"/> または <see cref="SkinnedMeshRenderer"/> を再帰的に取得する。
        /// </summary>
        /// <param name="root"></param>
        /// <returns>順序を持たずに列挙される<see cref="Renderer"/>。</returns>
        internal static IEnumerable<Renderer> GetConvertibleRenderersInChildren(GameObject root)
        {
            foreach (var smr in root.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                yield return smr;
            }

            foreach (var mr in root.GetComponentsInChildren<MeshRenderer>())
            {
                yield return mr;
            }
        }
    }
}
