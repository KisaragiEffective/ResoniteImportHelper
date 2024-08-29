using System.Collections.Generic;
using UnityEngine;

namespace ResoniteImportHelper.Editor
{
    internal static class GameObjectRecurseUtility
    {
        internal static IEnumerable<GameObject> GetChildrenRecursive(GameObject obj)
        {
            yield return obj;
            var c = obj.transform.childCount;
            for (var i = 0; i < c; i++)
            {
                foreach (var a in GetChildrenRecursive(obj.transform.GetChild(i).gameObject))
                {
                    // Debug.Log($"yielding {a}");
                    yield return a;
                }
            }
        }
    }
}