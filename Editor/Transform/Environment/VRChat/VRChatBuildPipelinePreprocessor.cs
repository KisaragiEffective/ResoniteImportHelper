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
    internal class VRChatBuildPipelinePreprocessor : IPlatformDependantPreprocessor
    {
        public GameObject Preprocess(GameObject modifiableRoot)
        {
#if RIH_HAS_VRCSDK3A
            if (!modifiableRoot.TryGetComponent<VRCAvatarDescriptor>(out _))
            {
                throw new Exception("specified object does not have VRChat Avatar Descriptor.");
            }

            VRCBuildPipelineCallbacks.OnPreprocessAvatar(modifiableRoot);
            return modifiableRoot;
#else
            throw new ResoniteImportHelper.Transform.Environment.Common.NonEnabledPlatformException();
#endif
        }
    }
}
