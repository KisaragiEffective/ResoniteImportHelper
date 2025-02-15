namespace ResoniteImportHelper.Package.Import.Metadata
{
    public interface IMetadataAccessor
    {
        public T GetAndDeserialize<T>(string key);
    }
}
