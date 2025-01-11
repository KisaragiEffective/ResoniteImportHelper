#nullable enable
using System.Collections.Generic;
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Lint
{
    internal interface ILintPass<out TDiagnostic> where TDiagnostic: IDiagnostic
    {
        public IEnumerable<TDiagnostic> Check(GameObject unmodifiableRoot);
    }
}
