#nullable enable
#if UNITY_EDITOR
using UnityEngine;

namespace ResoniteImportHelper.ClonedMarker
{
    [DisallowMultipleComponent]
    public sealed class IntermediateClonedHierarchyMarker : MonoBehaviour
    {
        [SerializeField]
        internal GameObject original;

        [SerializeField]
        internal bool automaticallyAdded;

        public static IntermediateClonedHierarchyMarker Construct(GameObject destination, GameObject original)
        {
            var x = destination.AddComponent<IntermediateClonedHierarchyMarker>();
            x.original = original;
            x.automaticallyAdded = true;

            return x;
        }
    }
}
#endif
