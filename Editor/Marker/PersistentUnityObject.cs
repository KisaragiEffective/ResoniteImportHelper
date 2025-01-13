using System;

namespace KisaragiMarine.ResoniteImportHelper.Marker.ResoniteImportHelper.Editor.Marker
{
    /// <summary>
    /// <list type="bullet">
    /// <item>メソッド及び戻り値に付けた場合: 戻り値が永続化されたオブジェクトであることを保証する。</item>
    /// <item>引数に付けた場合: 引数が永続化されたオブジェクトでなければならないことを意味する。</item>
    /// </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    public sealed class PersistentUnityObject : Attribute
    {

    }
}
