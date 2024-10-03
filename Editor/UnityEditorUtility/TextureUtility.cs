#nullable enable
using ResoniteImportHelper.Marker;
using UnityEngine;
using UnityEngine.Profiling;

namespace ResoniteImportHelper.UnityEditorUtility
{
    [NotPublicAPI]
    public static class TextureUtility
    {
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