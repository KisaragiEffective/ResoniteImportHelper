using Newtonsoft.Json;

namespace ResoniteImportHelper.Package.Import.Deserialize.Support
{
    public struct UniquePointer<T> : IIdentifiable
    {
        public string ID;
        [JsonProperty("Data")]
        public string ReferenceeID;

        public string GetIdentifier() => ID;
    }
}
