using UnityEngine;

namespace BbxCommon.Ui
{
    public static class UiApi
    {
        #region public
        public static T GetUiController<T>() where T : UiControllerBase
        {
            return UiControllerManager.GetUiController<T>();
        }

        public static void SetUiTop(GameObject uiGameObject)
        {
            UiControllerManager.SetUiTop(uiGameObject);
        }

        public static void SetUiTop(UiControllerBase uiController)
        {
            UiControllerManager.SetUiTop(uiController);
        }

        public static void SetTopUiBack(GameObject uiGameObject)
        {
            UiControllerManager.SetTopUiBack(uiGameObject);
        }

        public static void SetTopUiBack(UiControllerBase uiController)
        {
            UiControllerManager.SetTopUiBack(uiController);
        }
        #endregion

        #region internal
        internal static void SetUiGameEngineScene(UiGameEngineScene scene)
        {
            UiControllerManager.SetUiGameEngineScene(scene);
        }

        internal static UiGameEngineScene GetUiGameEngineScene()
        {
            return UiControllerManager.GetUiGameEngineScene();
        }

        internal static void CollectUiController<T>(T uiController) where T : UiControllerBase
        {
            UiControllerManager.CollectUiController(uiController);
        }

        internal static T GetPooledUiController<T>() where T : UiControllerBase
        {
            return UiControllerManager.GetPooledUiController<T>();
        }
        #endregion
    }
}
