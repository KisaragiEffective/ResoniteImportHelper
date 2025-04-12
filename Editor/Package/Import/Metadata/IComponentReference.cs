namespace ResoniteImportHelper.Package.Import.Metadata
{
    public interface IComponentReference<out T> where T: notnull
    {
        public int GetComponentTableIndex();

        public T GetComponentData();
    }
}
