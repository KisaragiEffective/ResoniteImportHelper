using System;
using Newtonsoft.Json;
using ResoniteImportHelper.Package.Import.Deserialize.Metadata;

namespace ResoniteImportHelper.Package.Import.Deserialize.Bitmap
{
    internal sealed class BitmapTag: IAssetTag {}

    [Serializable]
    internal sealed class BitmapMetadata : IMetadata<BitmapTag>
    {
        [JsonProperty("width")]
        internal uint Width;

        [JsonProperty("height")]
        internal uint Height;

        [JsonProperty("mipMapCount")]
        internal uint MipMapCount;

        /// <summary>
        /// Example. `png`
        /// </summary>
        [JsonProperty("baseFormat")]
        internal string Format;

        [JsonProperty("bitsPerPixel")]
        internal uint BitsPerPixel;

        [JsonProperty("channelCount")]
        internal uint ChannelCount;

        [JsonIgnore]
        internal uint BitsPerChannel => BitsPerPixel / ChannelCount;

        /// <summary>
        /// Example. `FullyOpaque`
        ///
        /// Example. `Alpha`
        /// </summary>
        [JsonProperty("alphaData")]
        internal string AlphaTreat;
    }
}
