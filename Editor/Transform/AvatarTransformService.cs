using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;
using ResoniteImportHelper.Allocator;
using ResoniteImportHelper.ClonedMarker;
using ResoniteImportHelper.Generic.Collections;
using ResoniteImportHelper.Transform.Environment.Common;
using ResoniteImportHelper.Transform.Environment.LilToon;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace ResoniteImportHelper.Transform
{
    internal static class AvatarTransformService
    {
        internal static Result PerformConversionPure(
            GameObject unmodifiableRoot,
            // ReSharper disable once InconsistentNaming
            bool runVRCSDKPipeline,
            // ReSharper disable once InconsistentNaming
            bool runNDMF,
            bool bakeTexture,
            ResourceAllocator alloc
        )
        {
            Profiler.BeginSample("EnvironmentDependantShallowCopyAndTransform");
            var target = Expand(unmodifiableRoot, runVRCSDKPipeline, runNDMF);
            Profiler.EndSample();
            
            var intermediateMarker = IntermediateClonedHierarchyMarker.Construct(target, unmodifiableRoot);

            var c = InPlaceConvert(target, bakeTexture, alloc);
            
            Object.DestroyImmediate(intermediateMarker);

            return new Result(target, c.Materials);
        }
        
        private static GameObject Expand(
            GameObject unmodifiableRoot,
            // ReSharper disable once InconsistentNaming
            bool runVRCSDKPipeline,
            // ReSharper disable once InconsistentNaming
            bool runNDMF
        )
        {
            GameObject modifiableRoot;

            if (runVRCSDKPipeline)
            {
                modifiableRoot = PerformEnvironmentDependantShallowCopy(new Environment.VRChat.VRChatBuildPipelineExpander(), unmodifiableRoot);
            }
            else if (runNDMF)
            {
                modifiableRoot = PerformEnvironmentDependantShallowCopy(new Environment.NDMF.StandaloneNDMFExpander(), unmodifiableRoot);
            }
            else
            {
                modifiableRoot = Object.Instantiate(unmodifiableRoot);
            }

            return modifiableRoot;

            GameObject PerformEnvironmentDependantShallowCopy(IPlatformExpander handler, GameObject localUnmodifiableRoot) =>
                handler.PerformEnvironmentDependantShallowCopy(localUnmodifiableRoot);
        }

        internal sealed class InPlaceConvertResult
        {
            internal readonly MultipleUnorderedDictionary<LoweredRenderMode, Material> Materials;

            internal InPlaceConvertResult(MultipleUnorderedDictionary<LoweredRenderMode, Material> materials)
            {
                Materials = materials;
            }
        }
        
        private static InPlaceConvertResult InPlaceConvert(GameObject target, bool bakeTexture, ResourceAllocator alloc)
        {
            var rig = FindRigSetting(target);
            if (rig == null)
            {
                throw new Exception(
                    "specified object does not have Animator component. did you remove it, or is it violating humanoid standards?");
            }

            Debug.Log("Automated NoIK processor");
            ModifyArmature(target, rig);
            Debug.Log("maybe bake texture");
            if (bakeTexture)
            {
                BakeTexture(target, alloc);
            }
            else
            {
                Debug.Log("Texture bake was skipped (disabled). Please turn on from experimental settings if you want to turn on.");
            }

            var materialMap = LowerShader(target, alloc);

            return new InPlaceConvertResult(materialMap);
        }

        [CanBeNull]
        private static Animator FindRigSetting(GameObject root) => 
            root.TryGetComponent<Animator>(out var a) ? a : null;

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
                if (b == null)
                {
#if RIH_HAS_VRCSDK3A
                    // 一部アバターはAnimatorではなくVRC-ADのみに目のボーンをアサインしていることがあるので気をつける
                    if (hbb is not (HumanBodyBones.LeftEye or HumanBodyBones.RightEye)) return;
                    
                    if (!root.TryGetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(out var ad))
                    {
                        return;
                    }

                    var eyeConfiguration = ad.customEyeLookSettings;
                    var left = eyeConfiguration.leftEye;
                    var right = eyeConfiguration.rightEye;

                    b = hbb is HumanBodyBones.LeftEye ? left : right;
                    if (b == null)
                    {
                        Debug.Log($"RewriteIfSet: {hbb.ToString()} is not set on Animator or VRC-AD");
                        return;
                    }
                    
                    Debug.Log($"RewriteIfSet: fallback from VRC Avatar Descriptor: {hbb.ToString()} = {b.name}");
#endif
                }
                
                b.gameObject.name = newName;
                touchedBone.Add(b.gameObject);
            }
            
            #region upper
            RewriteIfSet(HumanBodyBones.Hips, "Hips");
            RewriteIfSet(HumanBodyBones.Spine, "Spine");
            RewriteIfSet(HumanBodyBones.Chest, "Chest");
            RewriteIfSet(HumanBodyBones.Neck, "Neck");
            RewriteIfSet(HumanBodyBones.Head, "Head");
            // BipedRigがルーズすぎてUpperChestがChestとして認識される問題を回避するためにあっても無視する
            // RewriteIfSet(HumanBodyBones.UpperChest, "Upper Chest");
            
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
            foreach (var remainedBone in GameObjectRecurseUtility.GetChildrenRecursive(armatureRoot)
                         .Where(o => !touchedBone.Contains(o)))
            {
                // TODO: more smart NoIK flag
                remainedBone.name = $"<NoIK> {remainedBone.name}";
            }
        }

        private static void BakeTexture(GameObject root, ResourceAllocator allocator)
        {
            new LilToonHandler(allocator).PerformInlineTransform(root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="allocator"></param>
        /// <returns>Materials that considered to be transparent.</returns>
        private static MultipleUnorderedDictionary<LoweredRenderMode, Material> LowerShader(GameObject root, ResourceAllocator allocator)
        {
            Profiler.BeginSample("LowerShader");
            var outer = new MultipleUnorderedDictionary<LoweredRenderMode, Material>
                {
                    [LoweredRenderMode.Unknown] = new HashSet<Material>(),
                    [LoweredRenderMode.Opaque] = new HashSet<Material>(),
                    [LoweredRenderMode.Cutout] = new HashSet<Material>(),
                    [LoweredRenderMode.Blend] = new HashSet<Material>()
                };

            var loweredMaterialCache = new Dictionary<Material, ISealedLoweredMaterialReference>();
            
            foreach (var renderer in RendererUtility.GetConvertibleRenderersInChildren(root))
            {
                renderer.sharedMaterials = LowerShaderInner(renderer, loweredMaterialCache, outer, allocator);
            }
            Profiler.EndSample();

            return outer;
        }

        private static Material[] LowerShaderInner(
            Renderer renderer,
            Dictionary<Material, ISealedLoweredMaterialReference> loweredMaterialCache,
            MultipleUnorderedDictionary<LoweredRenderMode, Material> outer,
            ResourceAllocator allocator
        )
        {
            Profiler.BeginSample("LowerShaderInner.CachedLower");
            var materials = renderer
                .sharedMaterials
                .Select(m =>
                {
                    if (loweredMaterialCache.TryGetValue(m, out var cached))
                    {
                        Debug.Log($"LowerShader: cache hit: {m.name} -> {cached.GetMaybeConvertedMaterial()} ({cached.GetComputedRenderMode()})");
                        return cached;
                    }
                        
                    var lowered = LowerMaterialInline(m, allocator);
                        
                    loweredMaterialCache.Add(m, lowered);

                    return lowered;
                });
            Profiler.EndSample();

            Material[] materials2;
            
            try
            {
                Profiler.BeginSample("Allocate and collect");
                AssetDatabase.StartAssetEditing();
                materials2 = materials
                    .Aggregate(
                        (
                            Materials: new Material[renderer.sharedMaterials.Length],
                            Map: outer,
                            Counter: 0
                        ),
                        (acc, currentMaterial) =>
                        {
                            var aj = currentMaterial.GetAllocationJob();
                            if (aj != null)
                            {
                                var m = allocator.Save(aj.Value);
                            
                                acc.Materials[acc.Counter] = m;
                                acc.Map.Append(currentMaterial.GetComputedRenderMode(), m);
                            }
                            
                            return (acc.Materials, acc.Map, acc.Counter + 1);
                        }
                    ).Materials;
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                Profiler.EndSample();
            }

            return materials2;
        }
        
        private static ISealedLoweredMaterialReference LowerMaterialInline(Material m, ResourceAllocator alloc)
        {
            Profiler.BeginSample("LowerMaterialInline");
            var x = new LilToonHandler(alloc).LowerInline(m);
            Profiler.EndSample();
            return x;
        }

        internal sealed class Result
        {
            internal readonly GameObject Processed;
            internal readonly MultipleUnorderedDictionary<LoweredRenderMode, Material> Materials;

            internal Result(GameObject processed, MultipleUnorderedDictionary<LoweredRenderMode, Material> materials)
            {
                Processed = processed;
                Materials = materials;
            }
        }
    }
}
