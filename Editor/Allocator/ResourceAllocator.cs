using ResoniteImportHelper.Marker;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

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
            var basePath = BasePath + "/" + name;
            Debug.Log($"Allocating persistent asset: {typeof(T)} on {basePath}");
            T persistent;
            if (obj is Texture2D tex)
            {
                var path = $"{basePath}.png";
                var fw = tex.EncodeToPNG();
                var fullyQualifiedPath = $"{Application.dataPath}/../{path}";
                Debug.Log($"Note: Allocator saves given Texture2D as PNG file. This is non-avoidable behavior.\nDestination path: {fullyQualifiedPath}");
                
                File.WriteAllBytes(fullyQualifiedPath, fw);
                
                AssetDatabase.ImportAsset(path);

                persistent = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            else
            {
                var path = $"{basePath}.asset";
                AssetDatabase.CreateAsset(obj, path);
            
                persistent = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            
            Debug.Log($"Importer: {AssetDatabase.GetImporterType(basePath)}");

            return persistent;
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