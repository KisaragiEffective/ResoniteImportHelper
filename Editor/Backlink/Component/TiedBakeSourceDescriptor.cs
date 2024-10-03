#nullable enable
using UnityEngine;

namespace ResoniteImportHelper.Backlink.Component
{

    internal class TiedBakeSourceDescriptor : ScriptableObject
    {
        [SerializeField]
        internal GameObject? SerializedParent;
    }
}
