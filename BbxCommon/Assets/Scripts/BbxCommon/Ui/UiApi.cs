using System.Reflection;
using UnityEngine;

namespace BbxCommon.Ui
{
    public static class UiApi
    {
        #region UiController
        public static T GetUiController<T>() where T : UiControllerBase
        {
            return UiControllerManager.GetUiController<T>();
        }

        /// <summary>
        /// Getting type id of <see cref="UiControllerBase{TView}"/> through reflection. Recommends to cache the result.
        /// </summary>
        /// <param name="uiView"> A <see cref="UiViewBase"/> hangs on an exist UI proto <see cref="GameObject"/> or a prefab. </param>
        public static int GetUiControllerTypeId(UiViewBase uiView)
        {
            var type = typeof(UiControllerTypeId<>).MakeGenericType(uiView.GetControllerType());
            var method = type.GetMethod("GetId", BindingFlags.Static | BindingFlags.NonPublic);
            return (int)method.Invoke(null, null);
        }

        public static int GetUiControllerTypeId(UiControllerBase uiController)
        {
            return uiController.GetControllerTypeId();
        }

        public static int GetUiControllerTypeId<T>() where T : UiControllerBase
        {
            return UiControllerTypeId<T>.Id;
        }

        /// <summary>
        /// Open a <see cref="UiControllerBase"/> by getting out from the pool or creating a new one.
        /// The returned <see cref="UiControllerBase"/> is with base class type and set as invisible. If you need it to
        /// show on the screen, call <see cref="UiControllerBase.Show"/>.
        /// </summary>
        /// <param name="sourceView"> A <see cref="UiViewBase"/> hangs on an exist UI proto <see cref="GameObject"/> or a prefab. </param>
        /// <param name="uiControllerTypeId"></param>
        public static UiControllerBase OpenUiController(UiViewBase sourceView, int uiControllerTypeId, Transform parent)
        {
            // try getting controller from pool
            var uiController = UiControllerManager.GetPooledUiController(uiControllerTypeId);
            if (uiController != null)
            {
                uiController.transform.SetParent(parent);
                uiController.Open();
                return uiController;
            }
            // or create one from the GameObject
            var uiGameObject = Object.Instantiate(sourceView.gameObject);
            uiGameObject.SetActive(false);
            uiGameObject.transform.SetParent(parent);
            uiController = CreateUiControllerOnGameObject<UiControllerBase>(uiGameObject);
            uiController.Open();
            return uiController;
        }

        /// <summary>
        /// Create a <see cref="UiControllerBase"/> from the proto <see cref="GameObject"/> or prefab.
        /// </summary>
        internal static TController CreateUiControllerOnGameObject<TController>(GameObject uiGameObject) where TController : UiControllerBase
        {
            var uiView = uiGameObject.GetComponent<UiViewBase>();
            if (uiView == null)
            {
                Debug.LogError("If you want to create a UI item through prefab, there must be a UiViewBase on the GameObject.");
                return null;
            }
            var uiController = (TController)uiGameObject.AddMissingComponent(uiView.GetControllerType());
            uiView.UiController = uiController;
            uiController.SetView(uiView);
            uiController.Init();
            uiController.Open();    // TODO: get controllers only by calling OpenUiController, and keep this function private
            return uiController;
        }
        #endregion

        #region UiTop
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
