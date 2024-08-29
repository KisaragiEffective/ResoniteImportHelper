using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
#if RIH_HAS_NDMF
using nadena.dev.ndmf;
#endif
#if RIH_HAS_UNI_GLTF
using UniGLTF;
#endif
using UnityEditor;
using UnityEngine;
#if RIH_HAS_VRCSDK3A
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase.Editor.BuildPipeline;
#endif
using Object = UnityEngine.Object;

namespace ResoniteImportHelper.Editor
{
    internal static class BusinessLogic
    {
        private static GameObject PerformConversionPure(
            GameObject _root,
            // ReSharper disable once InconsistentNaming
            bool runVRCSDKPipeline,
            // ReSharper disable once InconsistentNaming
            bool runNDMF
        )
        {
            var target = _root;
            if (runVRCSDKPipeline)
            {
#if RIH_HAS_VRCSDK3A
                if (!target.TryGetComponent<VRCAvatarDescriptor>(out _))
                {
                    throw new Exception("specified object does not have VRChat Avatar Descriptor.");
                }

                var cloned = Object.Instantiate(target);

                VRCBuildPipelineCallbacks.OnPreprocessAvatar(cloned);
                target = cloned;
#else
                throw new Exception("assertion error");
#endif
            }
            else if (runNDMF)
            {
#if RIH_HAS_NDMF
                target = AvatarProcessor.ProcessAvatarUI(target);
#else
                throw new Exception("assertion error");
#endif
            }
            else
            {
                target = Object.Instantiate(target);
            }

            var rig = FindRigSetting(target);
            if (rig == null)
            {
                throw new Exception(
                    "specified object does not have Animator component. did you remove it, or is it violating humanoid standards?");
            }

            Debug.Log("Automated NoIK processor");
            ModifyArmature(target, rig);
            return target;
        }

        internal static GameObject PerformConversion(
            GameObject _root,
            // ReSharper disable once InconsistentNaming
            bool runVRCSDKPipeline,
            // ReSharper disable once InconsistentNaming
            bool runNDMF
        )
        {
            var target = PerformConversionPure(_root, runVRCSDKPipeline, runNDMF);

            if (!ExternalServiceStatus.HasUniGLTF)
#pragma warning disable CS0162 // Unreachable code detected
            {
                throw new Exception("assertion error");
            }
#pragma warning restore CS0162 // Unreachable code detected
            
            Debug.Log("Exporting model as gLTF");
            var serialized = WriteGltfToAssetFolder(target);
            
            Debug.Log("done");
            // we can remove target because it is cloned in either way.
            Object.DestroyImmediate(target, false);
            return serialized;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Serialized object.</returns>
        private static GameObject WriteGltfToAssetFolder(GameObject target)
        {
#if RIH_HAS_UNI_GLTF
            //*
            var data = new ExportingGltfData();
            {
                var exportSettings = new GltfExportSettings
                {
                    // ???
                    DivideVertexBuffer = true
                };

                using var exporter = new gltfExporter(data, exportSettings);
                exporter.Prepare(target);
                exporter.Export();
            }

            {
                const string destinationFolder = "ZZZ_TemporalAsset";
                {
                    // ReSharper disable once InconsistentNaming
                    var maybeNewGUID = AssetDatabase.CreateFolder("Assets", destinationFolder);
                    if (maybeNewGUID != "")
                    {
                        Debug.Log($"Temporal asset folder was created. GUID: {maybeNewGUID}");
                    }
                }
                
                var rel =
                    $"{destinationFolder}/Run_{DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}.gltf";
                Debug.Log("dp: " + Application.dataPath);
                // dataPathはAssetsで終わることに注意！！
                var path = $"{Application.dataPath}/{rel}";
                
                #region UniGLTF.GltfExportWindow から引用したGLTFを書き出す処理
                // SPDX-SnippetBegin
                // SPDX-License-Identifier: MIT
                // SPDX-SnippetName: UniGLTF.GltfExportWindow
                // SPFX-SnippetCopyrightText: Copyright (c) 2020 VRM Consortium
                // SPFX-SnippetCopyrightText: Copyright (c) 2018 Masataka SUMI for MToon
                
                var (json, buffer0) = data.ToGltf(path);
                {
                    // write JSON without BOM
                    var encoding = new System.Text.UTF8Encoding(false);
                    File.WriteAllText(path, json, encoding);
                }

                {
                    // write to buffer0 local folder
                    var dir = Path.GetDirectoryName(path);
                    var bufferPath = Path.Combine(dir, buffer0.uri);
                    File.WriteAllBytes(bufferPath, data.BinBytes.ToArray());
                }
                // SPDX-SnippetEnd
                #endregion
                
                var assetsRelPath = $"Assets/{rel}";
                {
                    AssetDatabase.ImportAsset(assetsRelPath);
                    AssetDatabase.Refresh();
                }

                return AssetDatabase.LoadAssetAtPath<GameObject>(assetsRelPath);
            }
#else
            throw new Exception("assertion error: UniGLTF is not installed on the project.");
#endif
        }

        [CanBeNull]
        private static Animator FindRigSetting(GameObject root)
        {
            return root.TryGetComponent<Animator>(out var a) ? a : null;
        }

        /// <summary>
        /// See also: <a href="https://wiki.resonite.com/Humanoid_Rig_Requirements_for_IK">Wiki</a>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="rig"></param>
        private static void ModifyArmature(GameObject root, Animator rig)
        {
            if (!rig.isHuman) return;

            var touchedBone = new HashSet<GameObject>();

            void RewriteIfSet(HumanBodyBones hbb, string newName)
            {
                var b = rig.GetBoneTransform(hbb);
                if (b == null) return;
                
                b.gameObject.name = newName;
                touchedBone.Add(b.gameObject);
            }
            
            #region upper
            RewriteIfSet(HumanBodyBones.Hips, "Hips");
            RewriteIfSet(HumanBodyBones.Spine, "Spine");
            RewriteIfSet(HumanBodyBones.Chest, "Chest");
            RewriteIfSet(HumanBodyBones.Neck, "Neck");
            RewriteIfSet(HumanBodyBones.Head, "Head");
            RewriteIfSet(HumanBodyBones.UpperChest, "Upper Chest");
            
            #region left uppers
            RewriteIfSet(HumanBodyBones.LeftShoulder, "Shoulder.L");
            RewriteIfSet(HumanBodyBones.LeftUpperArm, "Upper_Arm.L");
            RewriteIfSet(HumanBodyBones.LeftLowerArm, "Lower_Arm.L");
            RewriteIfSet(HumanBodyBones.LeftHand, "Hand.L");
            
            #region fingers
            RewriteIfSet(HumanBodyBones.LeftThumbProximal, "Thumb1.L");
            RewriteIfSet(HumanBodyBones.LeftThumbIntermediate, "Thumb2.L");
            RewriteIfSet(HumanBodyBones.LeftThumbDistal, "Thumb3.L");
            
            RewriteIfSet(HumanBodyBones.LeftIndexProximal, "Index1.L");
            RewriteIfSet(HumanBodyBones.LeftIndexIntermediate, "Index2.L");
            RewriteIfSet(HumanBodyBones.LeftIndexDistal, "Index3.L");
            
            RewriteIfSet(HumanBodyBones.LeftMiddleProximal, "Middle1.L");
            RewriteIfSet(HumanBodyBones.LeftMiddleIntermediate, "Middle2.L");
            RewriteIfSet(HumanBodyBones.LeftMiddleDistal, "Middle3.L");
            
            RewriteIfSet(HumanBodyBones.LeftRingProximal, "Ring1.L");
            RewriteIfSet(HumanBodyBones.LeftRingIntermediate, "Ring2.L");
            RewriteIfSet(HumanBodyBones.LeftRingDistal, "Ring3.L");
            
            RewriteIfSet(HumanBodyBones.LeftLittleProximal, "Little1.L");
            RewriteIfSet(HumanBodyBones.LeftLittleIntermediate, "Little2.L");
            RewriteIfSet(HumanBodyBones.LeftLittleDistal, "Little3.L");
            #endregion
            #endregion
            
            #region right uppers
            RewriteIfSet(HumanBodyBones.RightShoulder, "Shoulder.R");
            RewriteIfSet(HumanBodyBones.RightUpperArm, "Upper_Arm.R");
            RewriteIfSet(HumanBodyBones.RightLowerArm, "Lower_Arm.R");
            RewriteIfSet(HumanBodyBones.RightHand, "Hand.R");
            
            #region fingers
            RewriteIfSet(HumanBodyBones.RightThumbProximal, "Thumb1.R");
            RewriteIfSet(HumanBodyBones.RightThumbIntermediate, "Thumb2.R");
            RewriteIfSet(HumanBodyBones.RightThumbDistal, "Thumb3.R");
            
            RewriteIfSet(HumanBodyBones.RightIndexProximal, "Index1.R");
            RewriteIfSet(HumanBodyBones.RightIndexIntermediate, "Index2.R");
            RewriteIfSet(HumanBodyBones.RightIndexDistal, "Index3.R");
            
            RewriteIfSet(HumanBodyBones.RightMiddleProximal, "Middle1.R");
            RewriteIfSet(HumanBodyBones.RightMiddleIntermediate, "Middle2.R");
            RewriteIfSet(HumanBodyBones.RightMiddleDistal, "Middle3.R");
            
            RewriteIfSet(HumanBodyBones.RightRingProximal, "Ring1.R");
            RewriteIfSet(HumanBodyBones.RightRingIntermediate, "Ring2.R");
            RewriteIfSet(HumanBodyBones.RightRingDistal, "Ring3.R");
            
            RewriteIfSet(HumanBodyBones.RightLittleProximal, "Little1.R");
            RewriteIfSet(HumanBodyBones.RightLittleIntermediate, "Little2.R");
            RewriteIfSet(HumanBodyBones.RightLittleDistal, "Little3.R");
            #endregion
            #endregion
            #endregion
            
            #region lower
            #region left lower
            RewriteIfSet(HumanBodyBones.LeftUpperLeg, "Upper_Leg.L");
            RewriteIfSet(HumanBodyBones.LeftLowerLeg, "Lower_Leg.L");
            RewriteIfSet(HumanBodyBones.LeftFoot, "Foot.L");
            RewriteIfSet(HumanBodyBones.LeftToes, "Toe.L");
            #endregion
            
            #region right lower
            RewriteIfSet(HumanBodyBones.RightUpperLeg, "Upper_Leg.R");
            RewriteIfSet(HumanBodyBones.RightLowerLeg, "Lower_Leg.R");
            RewriteIfSet(HumanBodyBones.RightFoot, "Foot.R");
            RewriteIfSet(HumanBodyBones.RightToes, "Toe.R");
            #endregion
            #endregion
            
            RewriteIfSet(HumanBodyBones.LeftEye, "Left Eye");
            RewriteIfSet(HumanBodyBones.RightEye, "Right Eye");
            RewriteIfSet(HumanBodyBones.Jaw, "Jaw");
            
            // We do not have to modify visemes, because generally the original avatar contains definition
            // for it and Resonite claims that it recognizes them well.
            // For more information, see https://wiki.resonite.com/Visemes
            
            // NoIK recursion
            GameObject armatureRoot;
            {
                var hips = rig.GetBoneTransform(HumanBodyBones.Hips);
                var candidate = hips.parent;
                armatureRoot = candidate == root.transform ? hips.gameObject : candidate.gameObject;
            }
            Debug.Log($"anim root: {root}");
            foreach (var remainedBone in GetChildrenRecursive(armatureRoot)
                         .Where(o => !touchedBone.Contains(o)))
            {
                // TODO: more smart NoIK flag
                remainedBone.name = $"<NoIK> {remainedBone.name}";
            }
        }

        private static IEnumerable<GameObject> GetChildrenRecursive(GameObject obj)
        {
            yield return obj;
            var c = obj.transform.childCount;
            for (var i = 0; i < c; i++)
            {
                foreach (var a in GetChildrenRecursive(obj.transform.GetChild(i).gameObject))
                {
                    // Debug.Log($"yielding {a}");
                    yield return a;
                }
            }
        }
    }
}
