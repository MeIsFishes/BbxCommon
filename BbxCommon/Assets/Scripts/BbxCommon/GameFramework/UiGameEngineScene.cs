using System.Collections.Generic;
using UnityEngine;
using BbxCommon.Ui;

namespace BbxCommon
{
    internal enum EUiGameEngine
    {
        Loading,
        Pooled,
        Top,
    }

    internal class UiGameEngineScene : UiSceneBase<EUiGameEngine>
    {
        #region Common
        protected override void OnSceneInit()
        {
            UiGroupWrapper.CreateUiGroupRoot(EUiGameEngine.Loading);
            UiGroupWrapper.CreateUiGroupRoot(EUiGameEngine.Pooled);
            UiGroupWrapper.CreateUiGroupRoot(EUiGameEngine.Top);
        }
        #endregion

        #region Pooled UI
        internal void PoolUiController(UiControllerBase uiController)
        {
            UiControllerWrapper.SetUiToGroup(uiController, EUiGameEngine.Pooled);
        }
        #endregion

        #region Set UI top
        private Dictionary<GameObject, Transform> m_OriginalParent = new Dictionary<GameObject, Transform>();

        internal void SetUiTop(GameObject uiGameObject)
        {
            m_OriginalParent.Add(uiGameObject, uiGameObject.transform.parent);
            UiControllerWrapper.SetUiToGroup(uiGameObject, EUiGameEngine.Top);
        }

        internal void SetTopUiBack(GameObject uiGameObject)
        {
            if (m_OriginalParent.TryGetValue(uiGameObject, out var originalParent))
            {
                var originalActive = uiGameObject.activeSelf;
                uiGameObject.SetActive(false);
                uiGameObject.transform.SetParent(originalParent);
                uiGameObject.SetActive(originalActive);
                m_OriginalParent.Remove(uiGameObject);
            }
        }
        #endregion
    }
}
