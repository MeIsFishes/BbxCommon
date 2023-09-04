using System;
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
        // indexed by type id
        private static List<UiCollection> m_UiCollections = new();

        private static UiCollection GetUiCollection(int typeId)
        {
            if (m_UiCollections.Count > typeId && m_UiCollections[typeId] != null)
                return m_UiCollections[typeId];
            else
            {
                if (m_UiCollections.Count <= typeId)
                    m_UiCollections.ModifyCount(typeId);
                if (m_UiCollections[typeId] == null)
                    m_UiCollections[typeId] = new();
                return m_UiCollections[typeId];
            }
        }

        internal static UiControllerBase GetUiController(int typeId)
        {
            var uiCollection = GetUiCollection(typeId);
            return uiCollection.GetUiController();
        }

        internal static T GetUiController<T>() where T : UiControllerBase
        {
            var uiCollection = GetUiCollection(ClassTypeId<UiControllerBase, T>.Id);
            return (T)uiCollection.GetUiController();
        }

        internal static void CollectUiController<T>(T uiController) where T : UiControllerBase
        {
            var uiCollection = GetUiCollection(uiController.View.GetControllerTypeId());
            uiCollection.CollectUiController(uiController);
            m_UiGameEngineScene.PoolUiController(uiController);
        }

        internal static void CollectUiController(UiControllerBase uiController)
        {
            var uiCollection = GetUiCollection(((IUiControllerTypeId)uiController).GetControllerTypeId());
            uiCollection.CollectUiController(uiController);
            m_UiGameEngineScene.PoolUiController(uiController);
        }

        internal static UiControllerBase GetPooledUiController(int typeId)
        {
            var uiCollection = GetUiCollection(typeId);
            return uiCollection.GetPooledUiController();
        }

        internal static T GetPooledUiController<T>() where T : UiControllerBase
        {
            return (T)GetUiCollection(ClassTypeId<UiControllerBase, T>.Id).GetPooledUiController();
        }

        internal static void OnUiOpen(UiControllerBase uiController)
        {
            var uiCollection = GetUiCollection(uiController.GetControllerTypeId());
            uiCollection.OnUiOpen(uiController);
        }

        internal static void OnUiOpen<T>(UiControllerBase uiController) where T : UiControllerBase
        {
            var uiCollection = GetUiCollection(ClassTypeId<UiControllerBase, T>.Id);
            uiCollection.OnUiOpen(uiController);
        }
        #endregion
    }

    internal class UiCollection
    {
        private List<UiControllerBase> m_UiControllers = new();
        private List<UiControllerBase> m_PooledControllers = new();

        internal UiControllerBase GetUiController()
        {
            if (m_UiControllers.Count == 1)
                return m_UiControllers[0];
            else if (m_UiControllers.Count > 1)
            {
                Debug.LogError("There are more than 1 " + m_UiControllers[0].GetType().Name + " in the UiScene. In that case you cannot get the UiController in this way!");
                return null;
            }
            return null;
        }

        internal void CollectUiController(UiControllerBase uiController)
        {
            var index = m_UiControllers.IndexOf(uiController);
            m_UiControllers.UnorderedRemoveAt(index);
            m_PooledControllers.Add(uiController);
            uiController.gameObject.SetActive(false);
        }

        internal UiControllerBase GetPooledUiController()
        {
            if (m_PooledControllers.Count > 0)
            {
                var res = m_PooledControllers[m_PooledControllers.Count - 1];
                m_PooledControllers.RemoveAt(m_PooledControllers.Count - 1);
                res.gameObject.SetActive(true);
                return res;
            }
            return null;
        }

        internal void OnUiOpen(UiControllerBase uiController)
        {
            m_UiControllers.Add(uiController);
        }
    }
}
