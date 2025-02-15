using System;
using ResoniteImportHelper.Package.Import.Deserialize.Support;

namespace ResoniteImportHelper.Editor.Package.Import.Stub
{
    public sealed class StaticTexture2D : IIdentifiable
    {
        public string ID;
        public IdentifiableDataCell<bool> Enabled;
        public IdentifiableDataCell<string> URL;
        public IdentifiableDataCell<bool> IsNormalMap;
        public IdentifiableDataCell<string> WrapModeU;
        public IdentifiableDataCell<string> WrapModeV;
        public IdentifiableDataCell<bool> CrunchCompressed;
        public IdentifiableDataCell<string> MipMapFilter;

        public string GetAssetIdentifier()
        {
            const string PACKDB_PREFIX = "@packdb:///";
            var url = URL.Data;
            if (!url.StartsWith("@")) throw new Exception("URL does not start with at-mark.");
            if (!url.StartsWith(PACKDB_PREFIX)) throw new Exception("URL does not start with packdb prefix.");

            return url[PACKDB_PREFIX.Length..];
        }

        public string GetIdentifier() => ID;
    }
}
