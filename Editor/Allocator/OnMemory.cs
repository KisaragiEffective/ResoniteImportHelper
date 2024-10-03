#nullable enable
using ResoniteImportHelper.Marker;

namespace ResoniteImportHelper.Allocator
{
    [NotPublicAPI]
    public readonly struct InMemory<T>
    {
        public readonly T InMemoryValue;

        public InMemory(T value)
        {
            InMemoryValue = value;
        }
    }
}