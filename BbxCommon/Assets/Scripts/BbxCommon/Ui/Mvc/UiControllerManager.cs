using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    /// <summary>
    /// Offering internal interfaces of operating <see cref="UiControllerBase{TView}"/> but doesn't implement them directly, keeping interfaces clear and unchanged.
    /// </summary>
    internal static class UiControllerManager
    {
        #region UiGameEngineScene
        private static UiGameEngineScene m_UiGameEngineScene;

        internal static void SetUiGameEngineScene(UiGameEngineScene scene)
        {
            m_UiGameEngineScene = scene;
        }

        internal static UiGameEngineScene GetUiGameEngineScene()
        {
            return m_UiGameEngineScene;
        }

        internal static void SetUiTop(GameObject uiGameObject)
        {
            m_UiGameEngineScene.SetUiTop(uiGameObject);
        }

        internal static void SetUiTop(UiControllerBase uiController)
        {
            m_UiGameEngineScene.SetUiTop(uiController.gameObject);
        }

        internal static void SetTopUiBack(GameObject uiGameObject)
        {
            m_UiGameEngineScene.SetTopUiBack(uiGameObject);
        }

        internal static void SetTopUiBack(UiControllerBase uiController)
        {
            m_UiGameEngineScene.SetTopUiBack(uiController.gameObject);
        }
        #endregion

        #region UiCollection
        private static List<UiCollection> m_UiCollections = new();

        private static UiCollection GetUiCollection(int typeId)
        {
            if (m_UiCollections.Count <= typeId)
                m_UiCollections.ModifyCount(typeId);
            return m_UiCollections[typeId];
        }

        internal static T GetUiController<T>() where T : UiControllerBase
        {
            var uiCollection = GetUiCollection(UiControllerTypeId<T>.Id);
            return (T)uiCollection.GetUiController();
        }

        internal static void CollectUiController<T>(T uiController) where T : UiControllerBase
        {
            var uiCollection = GetUiCollection(UiControllerTypeId<T>.Id);
            uiCollection.CollectUiController(uiController);
        }

        internal static void CollectUiController(UiControllerBase uiController)
        {
            var uiCollection = GetUiCollection(((IUiControllerTypeId)uiController).GetControllerTypeId());
            uiCollection.CollectUiController(uiController);
        }

        internal static T GetPooledUiController<T>() where T : UiControllerBase
        {
            return (T)GetUiCollection(UiControllerTypeId<T>.Id).GetPooledUiController();
        }
        #endregion
    }

    internal class UiCollection
    {
        private List<UiControllerBase> m_UiControllers = new();
        private List<UiControllerBase> m_PooledControllers = new();

        internal UiControllerBase GetUiController()
        {
            if (m_UiControllers.Count > 1)
            {
                Debug.LogError("There are more than 1 " + typeof(T).Name + " in the UiScene. In that case you cannot get the UiController in this way!");
                return null;
            }
            return m_UiControllers[0];
        }

        internal void CollectUiController(UiControllerBase uiController)
        {
            m_PooledControllers.Add(uiController);
            uiController.ControllerInPool = true;
        }

        internal UiControllerBase GetPooledUiController()
        {
            if (m_PooledControllers.Count > 0)
            {
                var res = m_PooledControllers[m_PooledControllers.Count - 1];
                m_PooledControllers.RemoveAt(m_PooledControllers.Count - 1);
                res.ControllerInPool = false;
                return res;
            }
            return null;
        }
    }
}
