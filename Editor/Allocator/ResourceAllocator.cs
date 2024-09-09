using ResoniteImportHelper.Marker;
using UnityEditor;
using UnityEngine;

namespace ResoniteImportHelper.Allocator
{
    [NotPublicAPI]
    public class ResourceAllocator
    {
        private readonly string rootFolderGuid;
        
        [NotPublicAPI]
        public string BasePath => AssetDatabase.GUIDToAssetPath(rootFolderGuid);

        [NotPublicAPI]
        public ResourceAllocator(string guid)
        {
            this.rootFolderGuid = guid;
        }

        [NotPublicAPI]
        public T Save<T>(T obj, string name) where T : Object
        {
            var path = BasePath + "/" + name;
            AssetDatabase.CreateAsset(obj, path);
            
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        /// <summary>
        /// ファイルを不定の名前でセーブする。ファイル名を制御したい時は<see cref="Save{T}(T,string)"/>を使うこと。
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NotPublicAPI]
        public T Save<T>(T obj) where T : Object
        {
            return this.Save(obj, GUID.Generate().ToString());
        }
    }
}