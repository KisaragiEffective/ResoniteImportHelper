using System;
using Newtonsoft.Json;
using ResoniteImportHelper.Package.Import.Deserialize.Metadata;

namespace ResoniteImportHelper.Package.Import.Deserialize.Bitmap
{
    internal sealed class BitmapTag: IAssetTag {}

    [Serializable]
    public sealed class BitmapMetadata : IMetadata<BitmapTag>
    {
        [JsonProperty("width")]
        public uint Width;

        [JsonProperty("height")]
        public uint Height;

        [JsonProperty("mipMapCount")]
        public uint MipMapCount;

        /// <summary>
        /// Example. `png`
        /// </summary>
        [JsonProperty("baseFormat")]
        public string Format;

        [JsonProperty("bitsPerPixel")]
        public uint BitsPerPixel;

        [JsonProperty("channelCount")]
        public uint ChannelCount;

        [JsonIgnore]
        public uint BitsPerChannel => BitsPerPixel / ChannelCount;

        /// <summary>
        /// Example. `FullyOpaque`
        ///
        /// Example. `Alpha`
        /// </summary>
        [JsonProperty("alphaData")]
        public string AlphaTreat;
    }
}
