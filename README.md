# Resonite Import Helper
Bake and Import.

> [!WARNING]
> This revision is pre-release!
> You should not install this revision unless you know what you are doing and the purpose of this revision!

## Requirement and recommended tools
* UniGLTF is required to run. Download it from their [GitHub repository](https://github.com/vrm-c/UniVRM/releases).
* Git is required to install. Download it from [gitforwindows.org](https://gitforwindows.org/).
* Unity® is required to be 2022.3 series. Older or newer series may work, but it will **NOT** be supported.

Following tools are optional. This tool can invoke their hooks.
* VRChat® Avatar SDK ([Download](https://creators.vrchat.com/sdk/))
* Non-Destructive Modular Framework (NDMF)
  * AAO: Avatar Optimizer ([Download](https://vpm.anatawa12.com/avatar-optimizer/en/))
  * Modular Avatar ([Download](https://modular-avatar.nadena.dev/))

## What this does and does not
### Does
* Flag non-Rig bone as [`<NoIK>`][NOIK]
* Rename bones to be [Rig-detector friendly](https://wiki.resonite.com/Humanoid_Rig_Requirements_for_IK#Bone_Requirements)
* Serialize processed avatar as glTF format
* Call VRChat Avatar Build hooks
* Call NDMF plugins

### Does not
* Convert Material: Always exported as Standard Shader. This is technical limitation on UniGLTF. You may import them as XiexeToonShader (or any suitable built-in Shader).
* Convert Animation: Always ignored. You may want to reconstruct them by Protoflux after importing.

## How to use and import
> [!TIP]
> You may want to refer [article](https://dinosaur-fossil.hatenablog.com/entry/2024/09/11/215603) written in Japanese by Yoshi, community member.

### Install
#### UniGLTF
As described above, you have to install UniGLTF before starting.
RIH can download and configure it automatically, so you may skip this step:

![Bootstrapper UI](./Doc~/AutomatedInstallationUI.png)

This method is equivalent to install from their [GitHub release](https://github.com/vrm-c/UniVRM/releases/tag/v0.125.0) page.

#### RIH itself
Install this package via Package Manager.

As of writing, installing RIH requires Git to be installed. Other install method may be supported in the future.
Obtain Git installer from [gitforwindows.org](https://gitforwindows.org/) (or alternative proper and favorite method) if you do not have one.

After installed git, open your Project. Then:
1. Go to `Window > Package Manager`
2. Click `[+▼]`
3. Select "Add package from Git URL"
4. Type `https://github.com/KisaragiEffective/ResoniteImportHelper.git#0.1.15-packaging.0`

Snippet after `#` specifies revision to be installed. By this configuration, you specify latest tagged version. This is recommended style. Refer [Unity's manual](https://docs.unity3d.com/2022.3/Documentation/Manual/upm-git.html) to customize or install other version.

> [!WARNING]
> Again, this version of RIH is pre-release. Proceed if and only if you know what you are doing.

> [!WARNING]
> You may not want to refer un-tagged version. This is because doing so implies always fetch latest version, and it may have buggy code snippet.
> Tagged versions are slightly more stable.

### Convert
1. Go to `Tools > Resonite Import Helper`: \
    ![UI visual](./Doc~/r1.png)
2. Change language to Japanese if you prefer it.
3. Set avatar to be processed.
4. Configure "Export settings" if necessary. \
    Depending on your installation, following checkbox may change their state:
    * Invoke VRChat SDK preprocessor: Calls VRChat SDK preprocessor. \
      Implies "NDMF Manual Bake".
      This typically includes NDMF-based tools, VRCFury, etc. \
      This option cannot be used when the target does not have "VRC Avatar Descriptor".
    * NDMF Manual Bake: Calls NDMF Manual Bake. \
      This is useful when you are importing non-VRChat avatar.
5. Press "Start".
6. It will be processed. Usually this will take a few seconds.
7. The processed avatar appears on its field:\
    ![UI visual](./Doc~/r2.png)
8. Focus onto "Project" tab.
9. Click the processed avatar field. By doing, the Project tab focused to it.
10. Drag it to the Hierarchy. You can check if it does not look unexpectedly.
11. Press "Open in file system". File explorer will be pop up on top of screen.
12. Find a file ending with `.gltf` in the directory and drop it onto Resonite window.
13. Make sure every material keeps their looks.
14. Implement workaround or find alternative solution. The following Unity and Platform-specific components cannot be exported because glTF lacks corresponding concept:
    * **Animation**. Workaround: Configure ProtoFlux to toggle properties.
    * **Expression Menu**. Workaround: Set up [Context Menu](https://wiki.resonite.com/Category:Components:Radiant_UI:Context_Menu) to achieve similar effect.
    * Any Renderer that is not a SkinnedMeshRenderer nor a MeshRenderer.
    * Unity Constraints and VRC Constraints. Workaround: re-implement similar logic by Driving Slot's position, rotation, or scale.
    * FinalIK.
    * Dynamic Bone and VRC PhysBone. Workaround: Configure Dynamic Bone in Resonite.
    * VRC Contact.
    * VRC HeadChop.
    * VRC SpatialAudioSource.
    * VRC Station.
    * Particle System.
    * Rigidbody.

### Trouble shooting
#### false-positive NOIK
RIH recognizes [HUMANOID][UNITY-AVATAR] bones from [ANIMATOR COMPONENT]. Please re-check if necessary HUMANOID bones are assigned.

If it is made for VRChat, and has [VRChat Avatar Descriptor], additional logic applies:
* If [UNITY-AVATAR] does not have left eye bone or right eye bone, RIH pulls them from the Descriptor. \
  ![Describing figure](./Doc~/EyeBoneFallbackFromVRChatAvatarDescriptor.png)

#### false-negative NOIK
* If your avatar is not treated as [HUMANOID][UNITY-AVATAR], then RIH does not flag any bone [NOIK].
  * If your avatar is actually humanoid, please configure as being from [RIG-TAB].
  * This is technical limitation, because RIH can't determine which bone should be used as IK bone.

#### NOIK is not respected
It may be a known issue on Resonite side.

There is a report for hand bones, see https://github.com/Yellow-Dog-Man/Resonite-Issues/issues/1031 for more information.

#### semi-transparent texture is exported as opaque texture
This is implemented in release [0.1.13](https://github.com/KisaragiEffective/ResoniteImportHelper/releases/tag/0.1.13).

Please report bug if it does not solve your case.

[UNITY-AVATAR]: https://docs.unity3d.com/2022.3/Documentation/Manual/ConfiguringtheAvatar.html
[RIG-TAB]: https://docs.unity3d.com/2022.3/Documentation/Manual/FBXImporter-Rig.html
[ANIMATOR COMPONENT]: https://docs.unity3d.com/2022.3/Documentation/Manual/class-Animator.html
[NOIK]: https://wiki.resonite.com/Humanoid_Rig_Requirements_for_IK#Ignoring_Bones
[VRChat Avatar Descriptor]: https://creators.vrchat.com/avatars/creating-your-first-avatar#step-5---adding-an-avatar-descriptor

#### Converted model looks like far from original, what went wrong?
Typically, this is caused by Custom Shaders.

> [!NOTE]
> Definition: Custom Shader \
> Any Shader on Unity *except* Unity's Standard Shader is considered as Custom Shader. \
> This including, but not limited to lilToon, Poiyomi, and UTS.

By default (and most case), following property are kept:
* Albedo and its transparency
* Normal map

Please let me know if that's not true.

Supporting around Custom Shader-specific properties in is poor. 

However please remember that it will **never** be considered if you do not let me know.

Plus if you enabled "Bake lilToon's configuration into Texture" (behind Experimental Settings), Albedo is going to have those:
* Toon Correction
* 2nd / 3rd Main Texture
* Alpha Mask

It is not possible to cover all Custom Shaders. Because you may write one.

## Folder structure
There are a few file in the containing folder.

Containing folder can be found in `Assets/ZZZ_TemporalAsset/Run_{date}-{time}`.

* The file ending with `.gltf` - The converted glTF file. \
    The name is determined from your input. For example, Mafuyu will be exported as `Mafuyu.gltf`.
* Backlink-related files. These file will not be created if original hierarchy not a Prefab.
  * `serialized_local_modification.prefab` - Used for backlink. \
      Created if the Prefab instance has yet-saved [overrides](https://docs.unity3d.com/2022.3/Documentation/Manual/PrefabInstanceOverrides.html).
  * `tied.asset` - Contains backlink. Refers either `serialized_local_modification.prefab` or original Prefab.

![Describe](./Doc~/reference-graph.drawio.png)

## Versioning Policy
This project adopts ["Semantic Versioning 2.0.0"](https://semver.org/).

In addition to "Semantic Versioning 2.0.0", following rules applied:
* Each version component is increased by 1 in general release.
* If Major component is 0:
  * Minor component is increased and Patch component reset to 0 if:
      * Public API is changed in Not Backwards Compatible way
  * Patch component is increased if any of following happens:
      * New feature is implemented
      * Bug fix is applied
      * UI changes are made

Those are not counted as Public API:
* Any C# members (class, method, property, field, etc.) that:
  * cannot be accessed from outside the codebase without reflection and patching.
    * This includes `private`, `internal`, `private protected`, and `file` members.
  * marked with `[NotPublicAPI]`.
* Folder structure.
* Backlink component and its members.

This project may provide experimental feature or implementation.
It is allowed to be disappeared without marking it as Not Backwards Compatible.

## Disclaimer
glTF and the glTF logo are trademarks of the Khronos Group Inc.

Unity, the Unity logo, and all related names, logos, product and service names, designs, and slogans are registered trademarks of Unity Technologies or its subsidiaries.

VRChat and all related names are trademarks or registered trademarks of VRChat, Inc. (https://trademarks.justia.com/864/50/vrchat-86450229.html)

「VRM」は一般社団法人ＶＲＭコンソーシアムの商標又は登録商標です。 (登録6365806)

Developer nor commiter may NOT be associated with above organizations.
