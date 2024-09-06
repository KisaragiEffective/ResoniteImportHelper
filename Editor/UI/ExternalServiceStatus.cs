namespace ResoniteImportHelper.UI
{
    internal static class ExternalServiceStatus
    {
        internal const bool HasVRCSDK3A =
#if RIH_HAS_VRCSDK3A
            true
#else
            false
#endif
            ;
        internal const bool HasNDMF = 
#if RIH_HAS_NDMF
            true
#else
            false
#endif
            ;

        internal const bool HasUniGLTF =
#if RIH_HAS_UNI_GLTF
            true
#else
            false
#endif
            ;
    }
}