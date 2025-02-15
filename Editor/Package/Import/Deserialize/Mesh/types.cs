using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using ResoniteImportHelper.Package.Import.Deserialize.Metadata;
using ResoniteImportHelper.Package.Import.Deserialize.Shape;

namespace ResoniteImportHelper.Package.Import.Deserialize.Mesh
{
    internal sealed class MeshTag : IAssetTag {}

    [Serializable]
    internal sealed class MeshMetadata : IMetadata<MeshTag>
    {
        [JsonProperty("bones")]
        internal MeshBound Bounds;

        [JsonProperty("submeshMetadata")]
        internal List<SubMeshMetadata> SubMeshes;

        [JsonProperty("boneMetadata")]
        internal List<BoneMetadata> Bones;

        [JsonProperty("approximateBoneBounds")]
        internal List<ApproximateBoneBound> ApproximateBoneBound;
    }

    [Serializable]
    internal sealed class MeshBound
    {
        [JsonProperty("min")]
        internal Point3Mut Min;

        [JsonProperty("max")]
        internal Point3Mut Max;

        public override string ToString()
        {
            return $"({ShortenInfinityP(Min)})..({ShortenInfinityP(Max)})";

            string ShortenInfinityP(Point3Mut p) =>
                $"{ShortenInfinity(p.X)}, {ShortenInfinity(p.Y)}, {ShortenInfinity(p.Z)}";

            string ShortenInfinity(float v)
            {
                if (float.IsPositiveInfinity(v))
                {
                    return "+inf";
                }

                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (float.IsNegativeInfinity(v))
                {
                    return "-inf";
                }

                return v.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    [Serializable]
    internal sealed class SubMeshMetadata
    {
        [JsonProperty("elementCount")]
        internal int Polys;

        [JsonProperty("bounds")]
        internal MeshBound Bounds = null!;
    }

    [Serializable]
    internal sealed class BoneMetadata
    {
        [JsonProperty("weight0count")]
        internal int Weight0Count;

        [JsonProperty("weight1count")]
        internal int Weight1Count;

        [JsonProperty("weight2count")]
        internal int Weight2Count;

        [JsonProperty("weight3count")]
        internal int Weight3Count;

        [JsonProperty("bounds")]
        internal MeshBound Bounds = null!;
    }

    [Serializable]
    internal sealed class ApproximateBoneBound
    {
        [JsonProperty("rootBoneIndex")]
        internal int RootBoneIndex;

        [JsonProperty("bounds")]
        internal Sphere3dMut Bounds;
    }
}
