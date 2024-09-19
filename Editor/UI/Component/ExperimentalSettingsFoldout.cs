using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI
{
    internal sealed class ExperimentalSettingsFoldout: Foldout
    {
        internal readonly Toggle BakeShadersConfigurationIntoTextures;

        internal readonly Toggle GenerateIntermediateArtifact;

        internal ExperimentalSettingsFoldout()
        {
            this.value = false;
            this.text = "Experimental Settings";
            this.Add(
                new HelpBox("Experimental Settings may disappear at anytime, and break your avatar!\nUse at your own peril and wisely.", HelpBoxMessageType.Warning)
            );
            
            BakeShadersConfigurationIntoTextures = new Toggle("Bake lilToon's configuration into Texture")
            {
                tooltip = "Do you want to bake configuration such as AlphaMask?"
            };
            this.Add(BakeShadersConfigurationIntoTextures);

            GenerateIntermediateArtifact = new Toggle("Generate intermediate prefab")
            {
                tooltip = "Do you want to intermediate artifacts to debug?\nIt will be persisted under the same directory."
            };
            
            this.Add(GenerateIntermediateArtifact);
        }

    }
}