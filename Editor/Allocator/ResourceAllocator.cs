using System.IO;
using ResoniteImportHelper.Marker;
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
                var tex = MaybeDuplicateTexture(_tex); 
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
        
        /// <summary>
        /// see: <a href="https://discussions.unity.com/t/848617/2">Unity forum</a>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Texture2D MaybeDuplicateTexture(Texture2D source)
        {
            if (source.isReadable)
            {
                return source;
            }
            
            Profiler.BeginSample("MaybeDuplicateTexture");
            var renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            var previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            var readableTexture = new Texture2D(source.width, source.height);
            readableTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableTexture.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            Profiler.EndSample();

            return readableTexture;
        }
    }
}