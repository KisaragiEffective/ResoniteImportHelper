using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ResoniteImportHelper.Package.Import.Metadata
{
    [Serializable]
    public sealed class TypeRef: IEquatable<TypeRef>, IEqualityComparer<TypeRef>
    {
        public string Raw { get; }

        public TypeRef(string raw)
        {
            this.Raw = raw;
        }

        public TypeName Parse() => TypeName.Parse(this.Raw);

        public bool Equals(TypeRef other) => other != null && this.Raw == other.Raw;

        public bool Equals(TypeRef x, TypeRef y) =>
            (x, y) switch
            {
                (null, null) => true,
                (_, null) => false,
                (null, _) => false,
                (_, _) => x.Equals(y)
            };

        public int GetHashCode(TypeRef obj) => obj.Raw.GetHashCode();

        public override string ToString() => $"TypeRef({Raw})";
    }
}
