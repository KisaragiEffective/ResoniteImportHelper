using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using GUID = UnityEditor.GUID;
using Object = UnityEngine.Object;

namespace ResoniteImportHelper.UnityEditorUtility
{
    internal static class AssetDatabasePlusPlus
    {
        /// <summary>
        /// 与えられたオブジェクトがUnityEngineに組み込みのアセットかどうか確かめる。
        /// <br />
        /// 注意: 与えられたオブジェクトが組み込みの場合、それらのGUIDはZero-filledなのでGUIDによって識別することは不可能である。
        /// そのため、<see cref="UnityEngine.Object.name"/> で判別すること。
        /// </summary>
        /// <param name="obj">検査するオブジェクト</param>
        /// <returns>組み込みなら<c>true</c>、そうでなければ<c>false</c>。</returns>
        internal static bool IsUnityEngineBuiltinObject(Object obj) =>
            AssetDatabase.GetAssetPath(obj) == "Resources/unity_builtin_extra";

        internal static GUID ContainingFolder(Object obj)
        {
            // Project root relative. Usually starts with `Asset/` but not necessarily. (Packages/, Resources/, etc.)
            var path = AssetDatabase.GetAssetPath(obj);
            var parentDirectory = Directory.GetParent(path);
            var unityRelativePath = ExtractProjectRelativePathFromAbsolutePath(parentDirectory!.FullName);
            // Debug.Log($"getting {unityRelativePath}");
            
            return AssetDatabase.GUIDFromAssetPath(unityRelativePath);
        }

        internal static IEnumerable<DelayedReference<T>> FindSpecificAsset<T>(string[] findInFolder)
            where T : Object
        {
            return AssetDatabase
                .FindAssets($"t:{typeof(T).FullName}", findInFolder)
                .Select(guid => new DelayedReference<T>(guid));
        }

        internal static string ExtractProjectRelativePathFromAbsolutePath(string absolutePath)
        {
            var dp = Application.dataPath;
            Debug.Log($"dp: {dp}");

            return absolutePath[(dp.Length - "Assets".Length)..];
        }
    }

    internal class DelayedReference<T> where T : Object
    {
        private readonly string _guid;
        
        internal DelayedReference(string referenceGuid)
        {
            this._guid = referenceGuid;
        }

        internal T LoadFromAssetDatabase() => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_guid));
    }
}