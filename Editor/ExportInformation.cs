using UnityEngine;

namespace ResoniteImportHelper.Editor
{
    internal class ExportInformation
    {
        internal readonly GameObject SerializedObject;
        internal readonly bool HasVertexColor;

        internal ExportInformation(GameObject serializedObject, bool hasVertexColor)
        {
            SerializedObject = serializedObject;
            HasVertexColor = hasVertexColor;
        }
    }
}