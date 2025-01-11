#nullable enable
namespace KisaragiMarine.ResoniteImportHelper.Allocator
{
    public readonly struct InMemory<T>
    {
        public readonly T InMemoryValue;

        public InMemory(T value)
        {
            InMemoryValue = value;
        }
    }
}
