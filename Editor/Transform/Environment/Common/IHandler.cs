using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    internal interface IHandler
    {
        [NotPublicAPI]
        public GameObject PerformEnvironmentConversion(GameObject unmodifiableRoot);
    }
}
