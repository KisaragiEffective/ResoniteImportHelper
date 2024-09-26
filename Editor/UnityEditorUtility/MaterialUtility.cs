using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ResoniteImportHelper.UnityEditorUtility
{
    internal static class MaterialUtility
    {
        internal static Material CreateVariant(Material parent)
        {
            Debug.Log($"create material variant for {parent.name}");
            return new Material(parent)
            {
                parent = parent
            };
        }

        private static bool IsMaterialVariant(Material material)
        {
            return material.parent != null;
        }

        /// <summary>
        /// 与えられた<see cref="Material"/>がMaterial Variantであり、かつ何らかのoverrideを持っているかどうか検査する。
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        internal static bool HasAnyOverride(Material variant)
        {
            if (!IsMaterialVariant(variant))
            {
                return false;
            }
            
            // シェーダーは同じなのでプロパティの集合も同じ
            var parent = variant.parent;

            return MaterialEditor
                .GetMaterialProperties(new Object[] { variant })
                .Any(prop => !EqualsMaterialPropertyValue(GetPropertyValue(parent, prop), GetPropertyValue(variant, prop)));
        }

        private static bool EqualsMaterialPropertyValue(object lhs, object rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            switch (lhs)
            {
                case null:
                    return false;
                case ValueType ll when rhs is ValueType rr:
                    return Equals(ll, rr);
                case object[] ll when rhs is object[] rr:
                {
                    if (ll.Length != rr.Length)
                    {
                        return false;
                    }

                    return !ll.Where((t, i) => !EqualsMaterialPropertyValue(t, rr[i])).Any();
                }
                case Object unityLL when rhs is Object unityRR:
                    return unityLL == unityRR;
                case var _:
                    return lhs == rhs;
            }
        }

        private static object GetPropertyValue(Material material, MaterialProperty mp)
        {
            var name = mp.name;
            
            var x = (mp.type, mp.hasMixedValue) switch
            {
                (MaterialProperty.PropType.Color, true) => material.GetColorArray(name),
                (MaterialProperty.PropType.Color, false) => material.GetColor(name),            
                (MaterialProperty.PropType.Vector, true) => material.GetVectorArray(name),
                (MaterialProperty.PropType.Vector, false) => material.GetVector(name),          
                (MaterialProperty.PropType.Float, true) => material.GetFloatArray(name),            
                (MaterialProperty.PropType.Float, false) => material.GetFloat(name),            
                (MaterialProperty.PropType.Range, true) => ImpossibleCombination(),            
                (MaterialProperty.PropType.Range, false) => mp.rangeLimits,            
                (MaterialProperty.PropType.Texture, true) => ImpossibleCombination(),        
                (MaterialProperty.PropType.Texture, false) => material.GetTexture(name),        
                (MaterialProperty.PropType.Int, true) => ImpossibleCombination(),                
                (MaterialProperty.PropType.Int, false) => material.GetInt(name),
                _ => throw new ArgumentOutOfRangeException()
            };

            return x;

            object ImpossibleCombination()
            {
                throw new ArgumentOutOfRangeException(nameof(mp));
            }
        }
    }
}