using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ResoniteImportHelper.Editor
{
    internal static class MeshUtility
    {
        internal static IEnumerable<Mesh> GetMeshes(GameObject go)
        {
            return GameObjectRecurseUtility.GetChildrenRecursive(go).SelectMany(o => SingleOrNone(
                o.GetComponent<SkinnedMeshRenderer>()?.sharedMesh ?? o.GetComponent<MeshFilter>().sharedMesh
            ));
        }

        private static IEnumerable<T> SingleOrNone<T>(T? value) where T : notnull
        {
            if (value != null)
            {
                yield return value;
            }
        }
    }
}