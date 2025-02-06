#nullable enable
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Backlink.Component
{

    internal class TiedBakeSourceDescriptor : ScriptableObject
    {
        [SerializeField]
        internal GameObject? SerializedParent;
    }
}
