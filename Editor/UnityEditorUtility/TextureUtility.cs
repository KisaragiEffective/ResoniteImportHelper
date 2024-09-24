using System.Linq;
using ResoniteImportHelper.Marker;
using UnityEngine;
using UnityEngine.Profiling;

namespace ResoniteImportHelper.UnityEditorUtility
{
    [NotPublicAPI]
    public static class TextureUtility
    {
        internal static bool HasAnyNonOpaquePixel(Texture2D t2) =>
            MaybeDuplicateTexture(t2).GetRawTextureData<Color32>().AsReadOnly().Any(c => c.a != 255);
        
        /// <summary>
        /// see: <a href="https://discussions.unity.com/t/848617/2">Unity forum</a>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [NotPublicAPI]
        public static Texture2D MaybeDuplicateTexture(Texture2D source)
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