#nullable enable
using ResoniteImportHelper.Marker;

namespace ResoniteImportHelper.Lint
{
    [NotPublicAPI]
    public interface IDiagnostic
    {
        [NotPublicAPI]
        public string Message();
    }
}