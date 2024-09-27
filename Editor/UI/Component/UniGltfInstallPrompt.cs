using JetBrains.Annotations;
using ResoniteImportHelper.UI.Localize;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI.Component
{
    internal sealed class UniGltfInstallPrompt: VisualElement
    {
        internal UniGltfInstallPrompt([UsedImplicitly] ILocalizedTexts lang)
        {
#if !RIH_HAS_UNI_GLTF
            var rootVisualElement = this;
            
            var message =
#if RIH_HAS_UNI_GLTF_ANY
                lang.ConflictingVersionOfUniGLTFIsInstalled()
#else
                lang.UniGLTFIsNotInstalled()
#endif
                ;
            rootVisualElement.Add(
                new HelpBox(message, HelpBoxMessageType.Error)
            );
            {
                var button = new Button(() =>
                {
                    Application.OpenURL("https://github.com/vrm-c/UniVRM/releases/tag/v0.125.0");
                });
                button.Add(new Label(lang.OpenInstallationPageForUniGLTF()));
                rootVisualElement.Add(button);
            }
            {
                var button = new Button(PackageManagerProxy.InstallUniGLTF);
                button.Add(new Label(lang.InstallUniGLTFAutomatically()));
                rootVisualElement.Add(button);
            }
#endif
        }
    }
}