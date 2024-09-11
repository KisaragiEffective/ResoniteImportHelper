using nadena.dev.ndmf;
using ResoniteImportHelper.Transform.Environment.Common;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.NDMF
{
    internal class StandaloneNDMFExpander: IPlatformExpander
    {
        
        public GameObject PerformEnvironmentDependantShallowCopy(GameObject unmodifiableRoot)
        {
#if RIH_HAS_NDMF
            return AvatarProcessor.ProcessAvatarUI(unmodifiableRoot);
#else
            throw new ResoniteImportHelper.Transform.Environment.Common.NotEnabledPlatformException();
#endif
        }
    }
}