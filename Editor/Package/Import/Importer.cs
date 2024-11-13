#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using ResoniteImportHelper.Package.Import.Deserialize.Metadata;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ResoniteImportHelper.Package.Import
{
    [ScriptedImporter(1, "resonitepackage")]
    internal class Importer : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // TODO: adjust this later
            var root = new GameObject();
            ctx.AddObjectToAsset("$$main", root);
            ctx.SetMainObject(root);

            using var zipArchive = ZipFile.OpenRead(ctx.assetPath);

            var mainRecordEntry = zipArchive.GetEntry("R-Main.record");
            if (mainRecordEntry == null)
            {
                throw new FormatException("ResonitePackage must contain R-Main.record under the archive root.");
            }

            {
                // TODO: validate only -> UTF-8 only JSON parser to faster parse?
                var recordManifest = ReadZipArchiveContentAsUtf8Sequence(mainRecordEntry);
                Debug.Log($"root decoded: {recordManifest}");

                var pd = JsonConvert.DeserializeObject<PartialDescriptor>(recordManifest);
                if (pd is null)
                {
                    throw new FormatException("Failed to deserialize descriptor");
                }

                var manifests = pd.AssetManifest;

                var metadataArchiveEntries = zipArchive.Entries.Where(a => a.FullName.StartsWith("Metadata/")).ToList();
                var mainArchiveEntries = zipArchive.Entries.Where(a => a.FullName.StartsWith("Assets/")).ToList();

                var mainDataTreeEntry =
                    mainArchiveEntries.SingleOrDefault(e => e.Name == pd.AssetUri.Replace("packdb:///", ""));

                if (mainDataTreeEntry is null)
                {
                    throw new FormatException($"Failed to find main DataTreeDirectory: {pd.AssetUri} is not contained by the package.");
                }

                var mainDataTree = ReadZipArchiveContent(mainDataTreeEntry);

            }
        }

        private static byte[] ReadZipArchiveContent(ZipArchiveEntry entry)
        {
            var content = new byte[entry.Length];
            {
                using var meta = entry.Open();
                using var s = new BufferedStream(meta);

                s.Read(content);
            }

            return content;
        }

        private string ReadZipArchiveContentAsUtf8Sequence(ZipArchiveEntry entry)
        {
            return Encoding.UTF8.GetString(ReadZipArchiveContent(entry));
        }

        private bool TryReadAs<T>(string metadataJson, out T? meta) where T: IMetadata<IAssetTag>
        {
            var meshMeta = JsonConvert.DeserializeObject<T>(metadataJson);
            if (meshMeta == null)
            {
                meta = default;
                return false;
            }

            meta = meshMeta;
            return true;
        }

        internal sealed class FormatException : Exception
        {
            internal FormatException(string message) : base(message) {}
        }

        [Serializable]
        internal sealed class PartialDescriptor
        {
            [JsonProperty("assetUri")]
            internal string AssetUri;
            [JsonProperty("assetManifest")]
            internal List<AssetStatistic> AssetManifest;
        }

        [Serializable]
        internal sealed class AssetStatistic
        {
            [JsonProperty("hash")]
            internal string Hash;
            [JsonProperty("bytes")]
            internal int Length;
        }
    }
}
