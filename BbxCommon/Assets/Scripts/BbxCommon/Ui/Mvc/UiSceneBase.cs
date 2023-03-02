using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    public abstract class UiSceneBase<TGroupKey> : MonoBehaviour where TGroupKey : Enum
    {
        #region Common
        public GameObject RootProto;
        #endregion

        #region UiGroup
        protected Dictionary<TGroupKey, GameObject> m_UiGroups = new Dictionary<TGroupKey, GameObject>();

        public GameObject CreateUiGroupRoot(TGroupKey uiGroup, string name = "")
        {
            var root = Instantiate(RootProto);
            if (name.IsNullOrEmpty())
                root.name = uiGroup.ToString();
            else
                root.name = name;
            root.transform.parent = this.transform;
            m_UiGroups[uiGroup] = root;
            return root;
        }

        public TController OpenUiWithController<TController>(string path, TGroupKey uiGroup, bool open = true) where TController : UiControllerBase
        {
            var uiGameObject = Resources.Load<GameObject>(path);
            GameObject root;
            if (m_UiGroups.TryGetValue(uiGroup, out root) == false)
                CreateUiGroupRoot(uiGroup);
            uiGameObject.transform.parent = root.transform;
            if (uiGameObject.TryGetComponent<TController>(out var controller) == false)
                Debug.LogError("There is not a" + typeof(TController).ToString() + "on prefab's root GameObject, that's unexpected!");
            return controller;
        }

        public void SetUiGroup(List<TGroupKey> groups)
        {
            foreach (var pair in m_UiGroups)
            {
                if (groups.Contains(pair.Key))
                    pair.Value.SetActive(true);
                else
                    pair.Value.SetActive(false);
            }
        }

        public void SetUiGroup(params TGroupKey[] groups)
        {
            var list = SimplePool<List<TGroupKey>>.Alloc();
            list.AddArray(groups);
            SetUiGroup(list);
            list.CollectToPool();
        }
        #endregion

        #region UiModel
        protected Dictionary<Type, UiModelBase> m_UiModels = new Dictionary<Type, UiModelBase>();

        public void AddUiModel<T>(T model) where T : UiModelBase
        {
            m_UiModels.Add(typeof(T), model);
        }

        public void TryGetUiModel<T>(out T model) where T : UiModelBase
        {
            var t = typeof(T);
            if (m_UiModels.ContainsKey(t))

                model = (T)m_UiModels[t];
            else
                model = null;
        }

        public T TryGetUiModel<T>() where T : UiModelBase
        {
            TryGetUiModel(out T res);
            return res;
        }
        #endregion
    }
}
