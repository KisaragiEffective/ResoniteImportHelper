using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ResoniteImportHelper.Package.Import.Metadata
{
    public interface IAssetVariantAccessor
    {
        public IEnumerable<LazyLoadEntry> GetByKey(string Key);
    }

    public sealed class LazyLoadEntry
    {
        public readonly string FileName;
        private ZipArchiveEntry _entry;

        public LazyLoadEntry(ZipArchiveEntry entry)
        {
            _entry = entry;
            FileName = entry.Name;
        }

        public byte[] Decompress()
        {
            var len = this._entry.Length;
            if (len > int.MaxValue)
            {
                throw new Exception("too large entry");
            }

            using var ms = new MemoryStream();
            using (var w = _entry.Open())
            {
                w.CopyTo(ms);
            }

            return ms.GetBuffer();
        }
    }
}
