using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ResoniteImportHelper.Package.Import.Stub;

namespace ResoniteImportHelper.Package.Import.Metadata
{
    [Serializable]
    public sealed class GraphRoot
    {
        // ReSharper disable once InconsistentNaming
        public string VersionNumber;
        public Dictionary<string, int> FeatureFlags;
        [JsonProperty("Types")]
        private string[] _types;

        [JsonIgnore]
        private TypeRef[] _typesCache;

        [JsonIgnore]
        public TypeRef[] Types => (_typesCache ??= _types.Select(type => new TypeRef(type)).ToArray());

        [JsonProperty("TypeVersions")]
        private Dictionary<string, int> _typeVersions;

        [JsonIgnore]
        private Dictionary<TypeRef, int> _typeVersionsCache;

        [JsonIgnore]
        public Dictionary<TypeRef, int> TypeVersions =>
            _typeVersionsCache ??=
                _typeVersions
                    .Select(entry => KeyValuePair.Create(new TypeRef(entry.Key), entry.Value))
                    .ToDictionary(entry => entry.Key, entry => entry.Value);

        [JsonProperty("Object")]
        public Slot RootSlot;

        [JsonProperty("Assets")]
        public UntypedComponentReference[] ContainedAssets;
    }
}
