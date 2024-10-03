#nullable enable
#if UNITY_EDITOR
using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.ClonedMarker
{
    [NotPublicAPI]
    [DisallowMultipleComponent]
    public sealed class IntermediateClonedHierarchyMarker : MonoBehaviour
    {
        [SerializeField]
        internal GameObject original;

        [SerializeField]
        internal bool automaticallyAdded;

        [NotPublicAPI]
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
