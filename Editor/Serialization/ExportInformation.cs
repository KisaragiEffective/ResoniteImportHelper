#nullable enable
using System;
using System.Linq;
using ResoniteImportHelper.Backlink.Component;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEditor;
using UnityEngine;

namespace ResoniteImportHelper.Serialization
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

        internal TiedBakeSourceDescriptor LookupBacklink()
        {
            var secondary = AssetDatabasePlusPlus.ContainingFolder(this.SerializedObject);
            var path = AssetDatabase.GUIDToAssetPath(secondary);
            if (string.IsNullOrEmpty(path))
            {
                // 絶対パスを食わせたり何か気に食わないことがあったりしょうもないことで変な値を吐いて後続の処理が壊れるので
                // stopgapとして設置
                throw new Exception("programming error");
            }
            
            return AssetDatabasePlusPlus
                .FindSpecificAsset<TiedBakeSourceDescriptor>(new []{ path })
                .Single()
                .LoadFromAssetDatabase();
        }
    }
}