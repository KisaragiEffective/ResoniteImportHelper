using Newtonsoft.Json;

namespace ResoniteImportHelper.Package.Import.Metadata
{
    public sealed class TypedComponentReference<T> : IComponentReference<T> where T : notnull
    {
        [JsonProperty("Type")]
        public int ComponentTableIndex;
        [JsonProperty("Data")]
        public T ComponentProperties;

        public int GetComponentTableIndex() => ComponentTableIndex;

        public T GetComponentData() => ComponentProperties;
    }
}
