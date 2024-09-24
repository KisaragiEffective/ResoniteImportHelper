using System.Collections.Generic;
using ResoniteImportHelper.Marker;

namespace ResoniteImportHelper.Generic.Collections
{
    [NotPublicAPI]
    public sealed class MultipleUnorderedDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
        [NotPublicAPI]
        public void Append(TKey key, TValue value)
        {
            if (this.TryGetValue(key, out var preexistingValues))
            {
                preexistingValues.Add(value);
            }
            else
            {
                this.Add(key, new HashSet<TValue> { value });
            }
        }

        [NotPublicAPI]
        public void Delete(TKey key, TValue value)
        {
            if (!this.TryGetValue(key, out var values)) return;

            values.Remove(value);
            
            this.Add(key, values);
        }
    }
}