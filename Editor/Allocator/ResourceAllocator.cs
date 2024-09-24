using System.IO;
using ResoniteImportHelper.Marker;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

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
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path))
                {
                    Debug.Log($"Given instance has already serialized: {path}");
                    return obj;
                }
            }
            
            T persistent;
            if (obj is Texture2D _tex)
            {
                Profiler.BeginSample("Allocate Texture2D");
                var tex = TextureUtility.MaybeDuplicateTexture(_tex); 
                var path = $"{basePath}.png";
                Profiler.BeginSample("Encode To PNG");
                var fw = tex.EncodeToPNG();
                Profiler.EndSample();
                var fullyQualifiedPath = $"{Application.dataPath}/../{path}";
                Debug.Log($"Note: Allocator saves given Texture2D as PNG file. This is non-avoidable behavior.\nDestination path: {fullyQualifiedPath}");
                
                Profiler.BeginSample("Synchronous write");
                File.WriteAllBytes(fullyQualifiedPath, fw);
                Profiler.EndSample();
                
                Profiler.BeginSample("Tell Unity");
                AssetDatabase.ImportAsset(path);
                Profiler.EndSample();

                Profiler.BeginSample("Load");
                persistent = AssetDatabase.LoadAssetAtPath<T>(path);
                Profiler.EndSample();
            }
            else
            {
                Profiler.BeginSample("Allocate general asset");
                var path = $"{basePath}.asset";
                AssetDatabase.CreateAsset(obj, path);
            
                persistent = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            Profiler.EndSample();

            Debug.Log($"Importer: {AssetDatabase.GetImporterType(basePath)}");

            Debug.Assert(!AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(persistent)).Empty(), "object must be serialized on exit.");
            return persistent;
        }

        /// <summary>
        /// ファイルを不定の名前でセーブする。ファイル名を制御したい時は<see cref="Save{T}(T,string)"/>を使うこと。
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="UnityException">すでにアセットとして存在するものをシリアライズしようとした時。</exception>
        /// <returns></returns>
        [NotPublicAPI]
        public T Save<T>(T obj) where T : Object
        {
            return this.Save(obj, GUID.Generate().ToString());
        }
    }
}