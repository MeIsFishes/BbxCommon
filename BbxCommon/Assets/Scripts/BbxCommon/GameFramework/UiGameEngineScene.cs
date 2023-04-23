using System.Collections.Generic;
using UnityEngine;
using BbxCommon.Ui;

namespace BbxCommon
{
    internal enum EUiGameEngine
    {
        Loading,
        Top,
    }

    internal class UiGameEngineScene : UiSceneBase<EUiGameEngine>
    {
        protected override void OnSceneInit()
        {
            UiGroupWrapper.CreateUiGroupRoot(EUiGameEngine.Loading);
            UiGroupWrapper.CreateUiGroupRoot(EUiGameEngine.Top);
        }

        #region Set UI top
        private Dictionary<GameObject, Transform> m_OriginalParent = new Dictionary<GameObject, Transform>();

        internal void SetUiTop(GameObject uiGameObject)
        {
            m_OriginalParent.Add(uiGameObject, uiGameObject.transform.parent);
            var topTransform = GetUiGroupCanvas(EUiGameEngine.Top).transform;
            uiGameObject.transform.SetParent(topTransform);
        }

        internal void SetTopUiBack(GameObject uiGameObject)
        {
            if (m_OriginalParent.TryGetValue(uiGameObject, out var originalParent))
            {
                uiGameObject.transform.SetParent(originalParent);
                m_OriginalParent.Remove(uiGameObject);
            }
        }
        #endregion
    }
}
