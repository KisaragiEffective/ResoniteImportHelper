using UnityEditor;
using UnityEngine;

namespace ResoniteImportHelper.UnityEditorUtility
{
    internal static class AssetDatabasePlusPlus
    {
        /// <summary>
        /// 与えられたオブジェクトがUnityEngineに組み込みのアセットかどうか確かめる。
        /// <br />
        /// 注意: 与えられたオブジェクトが組み込みの場合、それらのGUIDはZero-filledなのでGUIDによって識別することは不可能である。
        /// そのため、<see cref="Object.name"/> で判別すること。
        /// </summary>
        /// <param name="obj">検査するオブジェクト</param>
        /// <returns>組み込みなら<c>true</c>、そうでなければ<c>false</c>。</returns>
        internal static bool IsUnityEngineBuiltinObject(Object obj) =>
            AssetDatabase.GetAssetPath(obj) == "Resources/unity_builtin_extra";
    }
}