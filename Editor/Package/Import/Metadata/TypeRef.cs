#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ResoniteImportHelper.Package.Import.Metadata
{
    [Serializable]
    public sealed class TypeRef: IEquatable<TypeRef>, IEqualityComparer<TypeRef>
    {
        public string Raw { get; }
        private bool? _hasCorrectSyntax;
        private Exception? _cachedException;
        private AssemblyName? _cachedAssemblyName;
        private TypeName? _cachedTypeName;

        public TypeRef(string raw)
        {
            this.Raw = raw;
        }

        public (AssemblyName assembly, TypeName typeName) Parse()
        {
            switch (this._hasCorrectSyntax)
            {
                case false:
                    throw this._cachedException!;
                case true:
                    return (this._cachedAssemblyName!, this._cachedTypeName!);
            }

            var regex = new Regex(@"^\[(?<assemblyName>[^\]]+)\](?<typeName>.+)$");
            var matchResult = regex.Match(this.Raw);
            if (matchResult.Success)
            {
                this._hasCorrectSyntax = true;
                var assemblyNameCaptureResult = matchResult.Groups["assemblyName"];
                if (!assemblyNameCaptureResult.Success)
                {
                    InvalidateAndThrow();
                }

                this._cachedAssemblyName = new AssemblyName(assemblyNameCaptureResult.Value);

                var typeNameCaptureResult = matchResult.Groups["typeName"];
                if (!typeNameCaptureResult.Success)
                {
                    InvalidateAndThrow();
                }

                if (!TypeName.TryParse(typeNameCaptureResult.Value, out var parsedTypeName))
                {
                    InvalidateAndThrow();
                }

                this._cachedTypeName = parsedTypeName;

                // Debug.Log($"asm: {_cachedAssemblyName.FullName}, ty: {_cachedTypeName.FullName}");
                return (_cachedAssemblyName, _cachedTypeName);
            }
            else
            {
                InvalidateAndThrow();
            }

            throw new InvalidOperationException("unreachable.");
        }

        [DoesNotReturn]
        private void InvalidateAndThrow()
        {
            this._hasCorrectSyntax = false;
            var exception = new Exception($"Malformed TypeRef: {Raw}");
            this._cachedException = exception;
            throw exception;
        }

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
