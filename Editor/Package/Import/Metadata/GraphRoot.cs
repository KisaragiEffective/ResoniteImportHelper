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
        public Dictionary<string, object> FeatureFlags;
        [JsonProperty("Types")]
        private string[] _types;

        private TypeRef[] _typesCache;
        public TypeRef[] OurTypes => (_typesCache ??= _types.Select(type => new TypeRef(type)).ToArray());

        [JsonProperty("TypeVersions")]
        private Dictionary<string, int> _typeVersions;
        private Dictionary<TypeRef, int> _typeVersionsCache;

        public Dictionary<TypeRef, int> OurTypeVersions =>
            _typeVersionsCache ??=
                _typeVersions
                    .Select(entry => KeyValuePair.Create(new TypeRef(entry.Key), entry.Value))
                    .ToDictionary(entry => entry.Key, entry => entry.Value);

        public Slot Object;
        public Dictionary<string, object>[] Assets;
    }
}
