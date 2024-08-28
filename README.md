# Resonite Import Helper
Bake and Import.

## Requirement and recommended tools
UniGLTF is required ([Download](https://github.com/vrm-c/UniVRM/releases))

Following tools are optional. This tool can invoke their hooks.
* VRChat Avatar SDK ([Download](https://creators.vrchat.com/sdk/))
* Non-Destructive Modular Framework
  * AAO: Avatar Optimizer ([Download](https://vpm.anatawa12.com/avatar-optimizer/en/))
  * Modular Avatar ([Download](https://modular-avatar.nadena.dev/))

## What this does and does not
### Does
* The [`<NoIK>`](https://wiki.resonite.com/Humanoid_Rig_Requirements_for_IK#Ignoring_Bones) flagging
* Renaming bones to be [Rig-detector friendly](https://wiki.resonite.com/Humanoid_Rig_Requirements_for_IK#Bone_Requirements)
* Serialize processed avatar as gLTF format
* Calls VRChat Avatar Build hooks
* Calls NDMF plugins

### Does not
* Material conversion: Always exported as Standard Shader. This is technical limitation on UniGLTF. You may import them as XiexeToonShader (or any suitable built-in Shader).
* Animation conversion: Always ignored. You may want to reconstruct them by Protoflux after importing.

## How to use and import
1. Install this package.
2. Install UniGLTF.
3. Go to `Tools > Resonite Import Helper`: ![UI visual](./Doc~/r1.png)
4. Set processing avatar.
5. Depending on your installation, following checkbox may change their state:
    * Invoke VRChat SDK preprocessor: Calls VRChat SDK preprocessor. \
      Implies "NDMF Manual Bake".
      This typically includes NDMF-based tools, VRCFury, etc. \
      This option cannot be used when the target does not have "VRC Avatar Descriptor".
    * NDMF Manual Bake: Calls NDMF Manual Bake. \
      This is useful when you are importing non-VRChat avatar.
6. Press "Start".
7. It will be processed. Usually this will take a few seconds.
8. The processed avatar appears on its field:![img_1.png](./Doc~/r2.png)
9. Press "Open in filesystem".
10. Drag and drop the outcome onto Resonite.
