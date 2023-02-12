using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    public class UiSceneBase<TGroupKey> : MonoBehaviour
    {
        #region UiGroup
        [SerializeField]
        protected Dictionary<TGroupKey, GameObject> m_UiGroups = new Dictionary<TGroupKey, GameObject>();

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
            var list = new List<TGroupKey>(groups);
            SetUiGroup(list);
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
