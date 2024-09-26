using ResoniteImportHelper.UI.Localize;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI.Component
{
    internal sealed class UniGltfInstallPrompt: VisualElement
    {
        internal UniGltfInstallPrompt(ILocalizedTexts lang)
        {
            var rootVisualElement = this;
#if !RIH_HAS_UNI_GLTF
            {
                rootVisualElement.Add(
                    new HelpBox(lang.UniGLTFIsNotInstalled(), HelpBoxMessageType.Error)
                );
                {
                    var button = new Button(() =>
                    {
                        Application.OpenURL("https://github.com/vrm-c/UniVRM/releases");
                    });
                    button.Add(new Label(lang.OpenInstallationPageForUniGLTF()));
                    rootVisualElement.Add(button);
                }
                {
                    var button = new Button(PackageManagerProxy.InstallUniGLTF);
                    button.Add(new Label(lang.InstallUniGLTFAutomatically()));
                    rootVisualElement.Add(button);
                }
            }
#endif
        }
    }
}