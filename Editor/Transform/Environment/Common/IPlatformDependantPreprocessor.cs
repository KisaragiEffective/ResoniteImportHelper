#nullable enable
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// プラットフォーム依存の方法で<see cref="GameObject"/>に対して変更を加える。
    /// </summary>
    public interface IPlatformDependantPreprocessor
    {
        /// <see cref="GameObject.Instantiate(Object)"/> を呼び出すときは必ず適切な後始末をしなければならない。
        /// <returns>変更が適用された<see cref="GameObject"/></returns>
        public GameObject Preprocess(GameObject modifiableRoot);
    }
}
