#if RIH_HAS_NDMF
using nadena.dev.ndmf;
#endif
using ResoniteImportHelper.Transform.Environment.Common;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.NDMF
{
    // ReSharper disable once InconsistentNaming
    internal class StandaloneNDMFExpander: IPlatformExpander
    {

        public GameObject PerformEnvironmentDependantShallowCopy(GameObject unmodifiableRoot)
        {
#if RIH_HAS_NDMF
            return AvatarProcessor.ProcessAvatarUI(unmodifiableRoot);
#else
            throw new ResoniteImportHelper.Transform.Environment.Common.NonEnabledPlatformException();
#endif
        }
    }
}
