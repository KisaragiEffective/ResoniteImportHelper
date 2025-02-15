using Newtonsoft.Json;
using ResoniteImportHelper.Package.Import.Deserialize.Support;
using UnityEngine;

namespace ResoniteImportHelper.Package.Import.Stub
{
    public sealed class Slot : IIdentifiable
    {
        public string ID;
        public IdentifiableDataCell<string> Name;

        [JsonProperty("Position")]
        private IdentifiableDataCell<float[]> _position;

        [JsonIgnore]
        private Vector3? _cachedPosition;

        [JsonIgnore] public Vector3 Position => _cachedPosition ??= new Vector3(_position.Data[0], _position.Data[1], _position.Data[2]);

        [JsonProperty("Rotation")]
        private IdentifiableDataCell<float[]> _rotation;

        [JsonIgnore]
        private Quaternion? _cachedRotation;

        [JsonIgnore] public Quaternion Rotation => _cachedRotation ??= new Quaternion(_rotation.Data[0], _rotation.Data[1], _rotation.Data[2], _rotation.Data[3]);

        [JsonProperty("Scale")]
        private IdentifiableDataCell<float[]> _scale;

        [JsonIgnore]
        private Vector3? _cachedScale;

        [JsonIgnore] public Vector3 Scale => _cachedScale ??= new Vector3(_scale.Data[0], _scale.Data[1], _scale.Data[2]);

        public Slot[] Children;
        public string GetIdentifier() => ID;
    }
}
