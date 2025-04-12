using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ResoniteImportHelper.Package.Import.Deserialize.Support
{
    public struct UniquePointer<T> : IIdentifiable
    {
        [UsedImplicitly]
        public string ID;

        [JsonProperty("Data")]
        public string ReferenceeID;

        public string GetIdentifier() => ID;
    }
}
