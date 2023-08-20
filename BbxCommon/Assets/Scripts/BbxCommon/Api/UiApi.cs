using System.Reflection;
using Unity.Entities;
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
            var type = typeof(ClassTypeId<,>).MakeGenericType(typeof(UiControllerBase), uiView.GetControllerType());
            var method = type.GetMethod("GetId", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return (int)method.Invoke(null, null);
        }

        public static int GetUiControllerTypeId(UiControllerBase uiController)
        {
            return uiController.GetControllerTypeId();
        }

        public static int GetUiControllerTypeId<T>() where T : UiControllerBase
        {
            return ClassTypeId<UiControllerBase, T>.Id;
        }

        /// <summary>
        /// <para>
        /// Recommended to be used for UI tools or library items only. In most cases, openning UI with interfaces offered in
        /// <see cref="GameEngineBase{TEngine}"/> and <see cref="GameStage"/> instead.
        /// </para><para>
        /// Open a <see cref="UiControllerBase"/> by getting out from the pool or creating a new one.
        /// The returned <see cref="UiControllerBase"/> is with base class type and set as invisible. If you need it to
        /// show on the screen, call <see cref="UiControllerBase.Show"/>.
        /// </para>
        /// </summary>
        /// <param name="sourceView"> A <see cref="UiViewBase"/> hangs on an exist UI proto <see cref="GameObject"/> or a prefab. </param>
        /// <param name="uiControllerTypeId"> For getting which type of <see cref="UiControllerBase{TView}"/> will be opened. Getting value
        /// via <see cref="GetUiControllerTypeId(UiViewBase)"/> or <see cref="GetUiControllerTypeId{T}"/>, and it is recommended to cache it. </param>
        public static UiControllerBase OpenUiController(UiViewBase sourceView, int uiControllerTypeId, Transform parent)
        {
            // try getting controller from pool
            var uiController = UiControllerManager.GetPooledUiController(uiControllerTypeId);
            if (uiController != null)
            {
                uiController.Open();
            }
            else
            {
                // or create one from the GameObject
                var uiGameObject = Object.Instantiate(sourceView.gameObject);
                uiGameObject.SetActive(false);

                uiController = CreateUiControllerWithGameObject(uiGameObject);
            }
            var transform = uiController.gameObject.AddMissingComponent<RectTransform>();
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return uiController;
        }

        /// <summary>
        /// Create a <see cref="UiControllerBase"/> from the proto <see cref="GameObject"/> or prefab.
        /// </summary>
        private static UiControllerBase CreateUiControllerWithGameObject(GameObject uiGameObject)
        {
            var uiView = uiGameObject.GetComponent<UiViewBase>();
            if (uiView == null)
            {
                Debug.LogError("If you want to create a UI item through prefab, there must be a UiViewBase on the GameObject.");
                return null;
            }
            var controllerGo = new GameObject(uiGameObject.name + "Controller");
            uiGameObject.transform.SetParent(controllerGo.transform);
            var uiController = (UiControllerBase)controllerGo.AddComponent(uiView.GetControllerType());
            uiView.UiController = uiController;
            uiController.SetView(uiView);
            uiController.Init();
            uiController.Open();
            return uiController;
        }
        #endregion

        #region UiModel
        public static void AddUiModel<T>() where T : UiModelBase, new()
        {
            UiModelManager.AddUiModel<T>();
        }

        public static T GetUiModel<T>() where T : UiModelBase
        {
            return UiModelManager.GetUiModel<T>();
        }

        public static void RemoveUiModel<T>() where T : UiModelBase
        {
            UiModelManager.RemoveUiModel<T>();
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

        #region HudController
        internal static GameObject HudRoot;

        /// <summary>
        /// <para>
        /// Recommended to be used for UI tools or library items only. In most cases, you can easily bind a <see cref="HudControllerBase{TView}"/>
        /// with an <see cref="Entity"/> via <see cref="UiApi.BindHud{T}(Entity)"/>.
        /// </para><para>
        /// Open a <see cref="HudControllerBase{TView}"/> by getting out from the pool or creating a new one.
        /// The returned <see cref="HudControllerBase{TView}"/> is with base class type and set as invisible. If you need it to
        /// show on the screen, call <see cref="UiControllerBase.Show"/>.
        /// </para>
        /// </summary>
        public static T OpenHudController<T>() where T : UiControllerBase, IHudController
        {
            // try getting controller from pool
            var hudController = UiControllerManager.GetPooledUiController(ClassTypeId<UiControllerBase, T>.Id) as T;
            if (hudController != null)
            {
                hudController.Open();
            }
            else
            {
                // or create one
                var controllerGameObject = new GameObject();
                hudController = controllerGameObject.AddComponent<T>();
                var hudGameObject = Object.Instantiate(Resources.Load<GameObject>(hudController.GetResourcePath()));
                hudGameObject.SetActive(false);
                hudGameObject.transform.SetParent(controllerGameObject.transform);
                controllerGameObject.name = hudGameObject.name;
                // attach view and controller each other
                var hudView = hudGameObject.GetComponent<HudViewBase>();
                hudView.UiController = hudController;
                hudController.View = hudView;
                // run life cycle
                hudController.Init();
                hudController.Open();
            }
            var transform = hudController.gameObject.AddMissingComponent<RectTransform>();
            transform.SetParent(HudRoot.transform);
            transform.anchorMin = Vector3.zero;     // function WorldToViewPort return the position relative to bottom left
            transform.anchorMax = Vector3.zero;
            transform.anchoredPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return hudController;
        }

        public static T BindHud<T>(this Entity entity) where T : UiControllerBase, IHudController
        {
            var hudController = OpenHudController<T>();
            hudController.Bind(entity);
            if (hudController.View.DefaultShow)
                hudController.Show();

            HudRawComponent hudComp;
            if (entity.HasRawComponent<HudRawComponent>())
                hudComp = entity.GetRawComponent<HudRawComponent>();
            else
                hudComp = entity.AddRawComponent<HudRawComponent>();
            hudComp.AddHudController(hudController);

            return hudController;
        }

        public static T GetHud<T>(this Entity entity) where T : UiControllerBase, IHudController
        {
            var hudComp = entity.GetRawComponent<HudRawComponent>();
            if (hudComp != null)
                return hudComp.GetHudController<T>();
            return null;
        }

        public static void RemoveHud<T>(this Entity entity) where T : UiControllerBase, IHudController
        {
            var hudComp = entity.GetRawComponent<HudRawComponent>();
            if (hudComp != null)
            {
                var hudController = hudComp.GetHudController<T>();
                hudController.Close();
                hudComp.RemoveHudController<T>();
            }
        }

        public static void ClearHud(this Entity entity)
        {
            var hudComp = entity.GetRawComponent<HudRawComponent>();
            if (hudComp != null)
            {
                foreach (var hud in hudComp.HudControllers)
                {
                    hud.Close();
                }
                hudComp.ClearHudControllers();
            }
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
