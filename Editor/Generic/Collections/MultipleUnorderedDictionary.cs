#nullable enable
using System.Collections.Generic;

namespace KisaragiMarine.ResoniteImportHelper.Generic.Collections
{
    public sealed class MultipleUnorderedDictionary<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
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

        public void Delete(TKey key, TValue value)
        {
            if (!this.TryGetValue(key, out var values)) return;

            values.Remove(value);

            this.Add(key, values);
        }
    }
}
