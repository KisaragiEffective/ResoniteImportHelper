#nullable enable
using System.Collections.Generic;
using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Lint
{
    internal interface ILintPass<out TDiagnostic> where TDiagnostic: IDiagnostic
    {
        [NotPublicAPI]
        public IEnumerable<TDiagnostic> Check(GameObject unmodifiableRoot);
    }
}
