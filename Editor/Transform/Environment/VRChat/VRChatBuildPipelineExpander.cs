#nullable enable
using System;
using UnityEngine;
#if RIH_HAS_VRCSDK3A
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase.Editor.BuildPipeline;
#endif
using Object = UnityEngine.Object;
using KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.VRChat
{
    internal class VRChatBuildPipelineExpander : IPlatformExpander
    {
        public GameObject PerformEnvironmentDependantShallowCopy(GameObject unmodifiableRoot)
        {
#if RIH_HAS_VRCSDK3A
            if (!unmodifiableRoot.TryGetComponent<VRCAvatarDescriptor>(out _))
            {
                throw new Exception("specified object does not have VRChat Avatar Descriptor.");
            }

            var cloned = Object.Instantiate(unmodifiableRoot);

            VRCBuildPipelineCallbacks.OnPreprocessAvatar(cloned);
            return cloned;
#else
            throw new ResoniteImportHelper.Transform.Environment.Common.NonEnabledPlatformException();
#endif
        }
    }
}
