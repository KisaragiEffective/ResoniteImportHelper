#nullable enable
#if RIH_HAS_NDMF
using nadena.dev.ndmf;
#endif
using KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common;
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.NDMF
{
    // ReSharper disable once InconsistentNaming
    internal class StandaloneNDMFPreprocessor: IPlatformDependantPreprocessor
    {

        public GameObject Preprocess(GameObject modifiableRoot)
        {
#if RIH_HAS_NDMF
            return AvatarProcessor.ProcessAvatarUI(modifiableRoot);
#else
            throw new ResoniteImportHelper.Transform.Environment.Common.NonEnabledPlatformException();
#endif
        }
    }
}
