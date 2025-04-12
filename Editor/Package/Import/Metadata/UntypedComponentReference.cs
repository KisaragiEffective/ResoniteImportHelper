using System.Collections.Generic;
using Newtonsoft.Json;

namespace ResoniteImportHelper.Package.Import.Metadata
{
    public sealed class UntypedComponentReference : IComponentReference<Dictionary<string, object>>
    {
        [JsonProperty("Type")]
        public int ComponentTableIndex;
        [JsonProperty("Data")]
        public Dictionary<string, object> UntypedComponentProperties;

        public int GetComponentTableIndex() => ComponentTableIndex;
        public Dictionary<string, object> GetComponentData() => UntypedComponentProperties;

        public TypedComponentReference<T> AsTyped<T>()
        {
            return new TypedComponentReference<T>
            {
                ComponentTableIndex = ComponentTableIndex,
                ComponentProperties = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(UntypedComponentProperties))
            };
        }
    }
}
