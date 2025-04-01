# Resonite Import Helper
Bake and Import.

## Requirement and recommended tools
* UniGLTF 0.128.0 is required to run. Download it from their [GitHub repository][unigltf-release].
* Git or VPM-compatible client is required to install.
    * Git: Download it from [git-scm.com](https://git-scm.com/downloads)
    * VPM-compatible client: You may use [ALCOM](https://vcc.docs.vrchat.com/guides/getting-started) or [VRChat Creator Companion](https://vcc.docs.vrchat.com/guides/getting-started).
* Unity® is required to be 2022.3 series. Older or newer series may work, but it will **NOT** be supported.

Following tools are optional. This tool can invoke their hooks.
* VRChat® Avatar SDK ([Download](https://creators.vrchat.com/sdk/))
* Non-Destructive Modular Framework (NDMF)
  * AAO: Avatar Optimizer ([Download](https://vpm.anatawa12.com/avatar-optimizer/en/))
  * Modular Avatar ([Download](https://modular-avatar.nadena.dev/))

[unigltf-release]: https://github.com/vrm-c/UniVRM/releases/tag/v0.128.0
## What this does and does not
### Does
* Flag non-Rig bone as [`<NoIK>`][NOIK]
* Rename bones to be [Rig-detector friendly](https://wiki.resonite.com/Humanoid_Rig_Requirements_for_IK#Bone_Requirements)
* Serialize processed avatar as [glTF](https://www.khronos.org/gltf/) format
* Call [VRChat Avatar Build hooks](https://creators.vrchat.com/sdk/build-pipeline-callbacks-and-interfaces/)
* Call [NDMF](https://ndmf.nadena.dev/) plugins

[NOIK]: https://wiki.resonite.com/Humanoid_Rig_Requirements_for_IK#Ignoring_Bones

### Does not
* Convert Material: Always exported as Standard Shader. This is technical limitation on UniGLTF. You may import them as XiexeToonShader (or any suitable built-in Shader).
* Convert Animation: Always ignored. You may want to reconstruct them by Protoflux after importing.

## Document
Please see [en/GETTING_STARTED.md](./Documentation~/en/user/getting_started/GETTING_STARTED.md).
