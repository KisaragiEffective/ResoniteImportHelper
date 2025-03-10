using System;
using Newtonsoft.Json;

namespace ResoniteImportHelper.Package.Import.Deserialize.Shape
{
    [Serializable]
    internal struct Sphere3dMut
    {
        [JsonProperty("center")]
        internal Point3Mut Center;

        [JsonProperty("radius")]
        internal float Radius;
    }
}
