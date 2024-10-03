#nullable enable
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ResoniteImportHelper.Serialization
{
    internal sealed class PostGltfService
    {
        private readonly dynamic _dynamicBinding;

        internal PostGltfService(string json)
        {
            _dynamicBinding = JsonConvert.DeserializeObject(json) ?? throw new Exception("Invalid JSON for PostGltfService");
        }

        internal void AlignInitialMorphValues(GameObject source)
        {
            var skinnedMeshRenderers = source.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var mesh in _dynamicBinding["meshes"])
            {
                string name = mesh["name"];
                JArray targetNames = mesh["extras"]["targetNames"];
                Debug.Log($"{nameof(AlignInitialMorphValues)}: checking {name}: has {targetNames.Count} morphs");
                if (targetNames.Count == 0)
                {
                    // weights cannot be empty, so write nothing.
                    continue;
                }

                var targetRenderer = skinnedMeshRenderers.SingleOrDefault(smr => smr.gameObject.name == name);

                if (targetRenderer == null)
                {
                    Debug.LogWarning($"{nameof(AlignInitialMorphValues)}: can't determine SkinnedMeshRenderers that correspond to {name}, skipping");
                    continue;
                }

                var m = targetRenderer.sharedMesh;

                var values = targetNames
                    .Cast<JValue>()
                    .Select(jv => jv.Value)
                    .Select(v => v!.ToString())
                    .Select(m.GetBlendShapeIndex)
                    .Select(targetRenderer.GetBlendShapeWeight)
                    .ToArray();

                var a = new JArray();

                foreach (var value in values)
                {
                    // Unity: 0.0 ~ 100.0
                    // glTF: 0.0 ~ 1.0
                    a.Add(value / 100.0);
                }

                mesh["weights"] = a;
            }
        }
    }
}
