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

This method is equivalent to install from their [GitHub release][unigltf-release] page.

#### RIH itself
Install can be done either:
* [From VPM-compatible index](#rih-itself-by-vpm-compatible-index) - recommended for non-tech user
* [From git](#rih-itself-by-git) - recommended for tech user and cutting-edge user

#### RIH itself by Git
Summary: Install this package via [Unity's Package Manager](https://docs.unity3d.com/2022.3/Documentation/Manual/upm-ui-giturl.html).

Obtain Git installer from [gitforwindows.org](https://gitforwindows.org/) (or alternative proper and favorite method) if you do not have one.

After installed git, open your Project.

> [!TIP]
> You do not have to create separate project. RIH is designed to be non-destructive.

On your project, do following:
1. Go to `Window > Package Manager`
2. Click `[+▼]`
3. Select "Add package from Git URL"
4. Type `https://github.com/KisaragiEffective/ResoniteImportHelper.git#0.1.19`

Snippet after `#` specifies revision to be installed. By this configuration, you specify latest tagged version. This is recommended style. Refer [Unity's manual](https://docs.unity3d.com/2022.3/Documentation/Manual/upm-git.html) to customize or install other version.

> [!WARNING]
> You may not want to refer un-tagged version. This is because doing so implies always fetch latest version, and it may have buggy code snippet.
> Tagged versions are slightly more stable.

#### RIH itself by VPM-compatible index
Summary: Add ksrgtech repository, then choose "Resonite Import Helper" (or `io.github.kisaragieffective.resonite-import-helper`).

Before installing, you have to add ksrgtech repository. Refer [VCC manual](https://vcc.docs.vrchat.com/guides/community-repositories/).

The client will request you to feed an URL which points to the repository. Type `https://raw.githubusercontent.com/ksrgtech/vpm-repository/refs/heads/live/index.json` to continue.

> [!TIP]
> For tech users: GitHub repository is published on <https://github.com/ksrgtech/vpm-repository>.

You might have to wait a moment. After that, find an item with name of "Resonite Import Helper".
I recommend to use latest non-pre-release version. As of writing, it is `0.1.17`, so please specify `0.1.17` from a pull-down.

> [!WARNING]
> For advanced users: pre-release is not suitable for your production environment!

> [!WARNING]
> Please take **back up Your Projects**, as always.

### Convert

> [!WARNING]
> The target avatar shall not have Missing Material on any renderer. \
> This is not supported: Those Material Slots will be ignored and be treated as the missing slot does not exist.
> Example:
>
> | # of order | Assigned Material |
> |:----------:|:-----------------:|
> |     1      |        Foo        |
> |     2      |     *Missing*     |
> |     3      |        Bar        |
>
> is treated as following:
>
> | # of order | Assigned Material |
> |:----------:|:-----------------:|
> |     1      |        Foo        |
> |     2      |        Bar        |
>

> [!WARNING]
> The target avatar shall not have Missing Script. This may supported in the future, but is not supported at this time.
> See [issue 168](https://github.com/KisaragiEffective/ResoniteImportHelper/issues/168) to learn why.

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
12. Find a file ending with `.gltf` in the directory.
13. The glTF will be loaded into the world. Its forward direction is negative Z if you are feeding a VRC-ready avatar and you do not modify root rotation.

## Import
You drop the outcome onto Resonite window.

You can choose several options on import. I do not write it here because it is off-topic from RIH usage. But please make sure every material keeps their looks to avoid reworking.

Finally, implement workaround or find alternative solution.
The following Unity and Platform-specific components cannot be exported because glTF lacks corresponding concept:
* **Animation**.
    * Workaround: Configure ProtoFlux to toggle properties.
* **Expression Menu**.
    * Workaround: Set up [Context Menu](https://wiki.resonite.com/Category:Components:Radiant_UI:Context_Menu) to achieve similar effect.
* Any Renderer that is not a SkinnedMeshRenderer nor a MeshRenderer.
    * Example: [Trail Renderer](https://docs.unity3d.com/2022.3/Documentation/Manual/class-TrailRenderer.html).
* Unity Constraints and VRC Constraints.
    * Workaround: re-implement similar logic by in-game Component and ProtoFlux nodes (thanks to Aetoriz's [post](https://misskey.resonite.love/notes/9ynqyi16tj)):
        * Look At Constraint and Aim Constraint map to [LookAt component][Component:LookAt].
        * Parent Constraint maps to [VirtualParent component][Component:VirtualParent].
        * Position Constraint and Rotation Constraint map to [CopyGlobalTransform component][Component:CopyGlobalTransform].
        * Scale Constraint maps to [CopyGlobalScale component][Component:CopyGlobalScale].
* FinalIK.
* Dynamic Bone and VRC PhysBone.
    * Workaround: Configure Dynamic Bone in Resonite.
* VRC Contact.
* VRC HeadChop.
* VRC SpatialAudioSource.
* VRC Station.
* Particle System.
* Rigidbody.

[Component:LookAt]: https://wiki.resonite.com/Component:LookAt
[Component:VirtualParent]: https://wiki.resonite.com/Component:VirtualParent
[Component:CopyGlobalTransform]: https://wiki.resonite.com/Component:CopyGlobalTransform
[Component:CopyGlobalScale]: https://wiki.resonite.com/Component:CopyGlobalScale

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

![Describe](./Doc~/reference-graph.png)

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
* Any C# members (class, method, property, field, etc.) that applicable to either:
  * cannot be accessed from outside the codebase without reflection and patching
    * This includes `private`, `internal`, `private protected`, and `file` members.
  * not marked with `[StableAPI]`
* Folder structure.
* Backlink component and its members.

This project may provide experimental feature or implementation.
It is allowed to be disappeared without marking it as Not Backwards Compatible.

## Disclaimer
glTF and the glTF logo are trademarks of the Khronos Group Inc.

Unity, the Unity logo, and all related names, logos, product and service names, designs, and slogans are registered trademarks of Unity Technologies or its subsidiaries.

VRChat and all related names are trademarks or registered trademarks of VRChat, Inc. (https://trademarks.justia.com/864/50/vrchat-86450229.html)

「VRM」は一般社団法人ＶＲＭコンソーシアムの商標又は登録商標です。 (登録6365806)

「resonite」は Yellow Dog Man Studios s.r.o. の商標または登録商標として出願または認可されています。([国際登録1748918](https://www.j-platpat.inpit.go.jp/c1801/TR/JP-1748918-20230601/49/ja))

Developer nor commiter may NOT be associated with above organizations.
