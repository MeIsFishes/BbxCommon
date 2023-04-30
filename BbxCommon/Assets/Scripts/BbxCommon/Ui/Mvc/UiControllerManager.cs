using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    /// <summary>
    /// Offering internal interfaces of operating <see cref="UiControllerBase{TView}"/> but doesn't implement them directly, keeping interfaces clear and unchanged.
    /// </summary>
    internal static class UiControllerManager
    {
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

        internal static T GetUiController<T>() where T : UiControllerBase
        {
            return UiCollection<T>.GetUiController();
        } 

        internal static void SetTopUiBack(UiControllerBase uiController)
        {
            m_UiGameEngineScene.SetTopUiBack(uiController.gameObject);
        }

        internal static void CollectUiController<T>(T uiController) where T : UiControllerBase
        {
            UiCollection<T>.CollectUiController(uiController);
        }

        internal static T GetPooledUiController<T>() where T : UiControllerBase
        {
            return UiCollection<T>.GetPooledUiController();
        }
    }

    internal static class UiCollection<T> where T : UiControllerBase
    {
        private static List<T> m_UiControllers = new();
        private static List<T> m_PooledControllers = new();

        internal static T GetUiController()
        {
            if (m_UiControllers.Count > 1)
            {
                Debug.LogError("There are more than 1 " + typeof(T).Name + " in the UiScene. In that case you cannot get the UiController in this way!");
                return null;
            }
            return m_UiControllers[0];
        }

        internal static void CollectUiController(T uiController)
        {
            m_PooledControllers.Add(uiController);
        }

        internal static T GetPooledUiController()
        {
            if (m_PooledControllers.Count > 0)
            {
                var res = m_PooledControllers[m_PooledControllers.Count - 1];
                m_PooledControllers.RemoveAt(m_PooledControllers.Count - 1);
                return res;
            }
            return null;
        }
    }
}
