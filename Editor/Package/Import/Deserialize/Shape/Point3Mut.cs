using System;
using Newtonsoft.Json;

namespace ResoniteImportHelper.Package.Import.Deserialize.Shape
{
    [Serializable]
    internal struct Point3Mut
    {
        [JsonProperty("x")]
        internal float X;

        [JsonProperty("y")]
        internal float Y;

        [JsonProperty("z")]
        internal float Z;
    }
}
