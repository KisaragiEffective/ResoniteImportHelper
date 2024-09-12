using System.Collections.Generic;
using UnityEngine;

namespace ResoniteImportHelper.UnityEditorUtility
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

        internal static void EnableAllChildrenWithRenderers(GameObject root)
        {
            foreach (var gameObject in GetChildrenRecursive(root))
            {
                if (gameObject.TryGetComponent(out SkinnedMeshRenderer smr))
                {
                    gameObject.SetActive(true);
                    EnableAllAncestor(gameObject);
                    smr.enabled = true;
                } else if (gameObject.TryGetComponent(out MeshRenderer mr))
                {
                    gameObject.SetActive(true);
                    EnableAllAncestor(gameObject);
                    mr.enabled = true;
                }
            }
        }

        private static void EnableAllAncestor(GameObject innermost)
        {
            foreach (var parentTransform in innermost.GetComponentsInParent<Transform>(true))
            {
                // Debug.Log($"enabling {parentTransform.name}");
                parentTransform.gameObject.SetActive(true);
            }
        }
    }
}