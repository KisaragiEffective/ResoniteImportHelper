using System;
using KisaragiMarine.ResoniteImportHelper.Marker.ResoniteImportHelper.Editor.Marker;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Search;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KisaragiMarine.ResoniteImportHelper.UI.Session
{
    /// <summary>
    /// コードのコンパイル前後で呼び出すことでUIの設定値を復元できるようにする。
    /// </summary>
    internal sealed class SessionStrider
    {
        internal static readonly IEditorPreferenceLens<GameObject> OriginalObject =
            new PersistentObjectEditorPreferenceLens<GameObject>(
                "ResoniteImportHelper-OriginalObject-f54ce5a6-5cb3-4edd-b883-67699d7305f1");

        internal static readonly IEditorPreferenceLens<GameObject> SerializedObject =
            new MaybeNotSerializedGameObjectEditorPreferenceLens();

        internal static readonly IEditorPreferenceLens<bool> UseVRChatBuildPipelineCallbacks =
                new BoolEditorPreferenceLens("ResoniteImportHelper-UseVRChatBuildPipelineCallbacks");

        internal static readonly IEditorPreferenceLens<bool> PerformNdmfManualBake =
            new BoolEditorPreferenceLens("ResoniteImportHelper-PerformNdmfManualBake");
    }

    internal interface IReadonlyEditorPreferenceLens
    {
        internal object GetBoxed();
    }

    internal interface IEditorPreferenceLens<TElem> : IReadonlyEditorPreferenceLens
    where TElem : notnull
    {
        object IReadonlyEditorPreferenceLens.GetBoxed()
        {
            return Get();
        }

        internal TElem? Get();
        internal void Set(TElem value);
    }

    internal sealed class BoolEditorPreferenceLens : IEditorPreferenceLens<bool>
    {
        private readonly string _key;

        internal BoolEditorPreferenceLens(string key)
        {
            this._key = key;
        }

        bool IEditorPreferenceLens<bool>.Get() => EditorPrefs.GetBool(_key);

        void IEditorPreferenceLens<bool>.Set(bool value)
        {
            EditorPrefs.SetBool(_key, value);
        }
    }

    internal sealed class PersistentObjectEditorPreferenceLens<TObject> : IEditorPreferenceLens<TObject>
        where TObject : Object
    {
        private readonly string _key;

        internal PersistentObjectEditorPreferenceLens(string key)
        {
            this._key = key;
        }

        [PersistentUnityObject]
        TObject IEditorPreferenceLens<TObject>.Get()
        {
            var s = EditorPrefs.GetString(this._key);

            return AssetDatabase.LoadAssetAtPath<TObject>(AssetDatabase.GUIDToAssetPath(s));
        }

        void IEditorPreferenceLens<TObject>.Set([PersistentUnityObject] TObject value)
        {
            EditorPrefs.SetString(this._key, AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(value)).ToString());
        }
    }

    internal enum GameObjectRecoverMethodKind: byte
    {
        Scene,
        AssetDatabase,
    }

    internal interface IGameObjectRecoverMethod
    {
        internal GameObject Get();

        internal GameObjectRecoverMethodKind GetTag();

        internal string MethodImplDependantObjectPointee();
    }

    internal sealed class FromSceneHierarchy : IGameObjectRecoverMethod
    {
        private readonly string _hierarchyPath;

        internal FromSceneHierarchy(string hierarchyPath)
        {
            this._hierarchyPath = hierarchyPath;
        }

        GameObject IGameObjectRecoverMethod.Get() => GameObject.Find(_hierarchyPath);
        GameObjectRecoverMethodKind IGameObjectRecoverMethod.GetTag() => GameObjectRecoverMethodKind.Scene;
        string IGameObjectRecoverMethod.MethodImplDependantObjectPointee() => _hierarchyPath;
    }

    internal sealed class FromAssetDatabase : IGameObjectRecoverMethod
    {
        private readonly GUID _assetGuid;

        internal FromAssetDatabase(UnityEditor.GUID assetGuid)
        {
            this._assetGuid = assetGuid;
        }

        GameObject IGameObjectRecoverMethod.Get()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(_assetGuid));
        }
        GameObjectRecoverMethodKind IGameObjectRecoverMethod.GetTag() => GameObjectRecoverMethodKind.AssetDatabase;
        string IGameObjectRecoverMethod.MethodImplDependantObjectPointee() => _assetGuid.ToString();
    }

    // TODO: EditorPrefsに保存する
    internal sealed class MaybeNotSerializedGameObjectEditorPreferenceLens : IEditorPreferenceLens<GameObject>
    {
        private IGameObjectRecoverMethod _method;

        internal MaybeNotSerializedGameObjectEditorPreferenceLens()
        {
        }

        GameObject IEditorPreferenceLens<GameObject>.Get()
        {
            return _method?.Get();
        }

        void IEditorPreferenceLens<GameObject>.Set(GameObject value)
        {
            var x = SearchUtils.GetTransformPath(value.transform);
            if (string.IsNullOrEmpty(x))
            {
                _method = new FromAssetDatabase(AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(value)));
            }
            else
            {
                _method = new FromSceneHierarchy(x);
            }
        }
    }
}
