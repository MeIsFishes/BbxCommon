using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    public abstract class UiSceneBase<TGroupKey> : MonoBehaviour where TGroupKey : Enum
    {
        #region Common
        public GameObject RootProto;
        [Tooltip("Export UiSceneAsset via UiSceneExporter and set to UiScene, then UiScene will create UI items stored in the asset when initializes.")]
        public UiSceneAsset SceneAsset;

        protected void Awake()
        {
            OnSceneInit();
            CreateUiByAsset(SceneAsset);
        }

        protected virtual void OnSceneInit() { }

        public UiControllerBase CreateUi(string path, TGroupKey uiGroup, bool defaultOpen = true)
        {
            var uiGameObject = Instantiate(Resources.Load<GameObject>(path));
            // hangs UI to group
            GameObject root;
            if (m_UiGroups.TryGetValue(uiGroup, out root) == false)
                CreateUiGroupRoot(uiGroup);
            uiGameObject.transform.parent = root.transform;
            // create controller and set view
            var uiView = uiGameObject.GetComponent<UiViewBase>();
            if (uiView == null)
            {
                Debug.LogError("If you want to create a UI item through prefab, there must be a UiViewBase on the GameObject.");
                return null;
            }
            var uiController = uiGameObject.AddMissingComponent(uiView.GetControllerType()) as UiControllerBase;
            uiController.SetView(uiView);
            // process defaultOpen
            if (defaultOpen)
                uiController.Open();
            else
                uiController.Close();
            // add to dictionary
            if (m_UiControllers.TryAdd(uiView.GetControllerType(), uiController) == false)
                Debug.LogError("You can't create a same UI twice! UI type: " + uiView.GetControllerType());
            return uiController;
        }

        protected void CreateUiByAsset(UiSceneAsset asset)
        {
            if (asset == null)
                return;
            foreach (var data in asset.UiObjectDatas)
            {
                var controller = CreateUi(data.PrefabPath, (TGroupKey)(object)data.UiGroup, false);
                (controller.transform as RectTransform).localPosition = data.Position;
                (controller.transform as RectTransform).localScale = data.Scale;
                (controller.transform as RectTransform).pivot = data.Pivot;
                if (data.DefaultOpen)   // keep OnUiOpen() calls after setting data
                    controller.Open();
            }
        }
        #endregion

        #region UiControllers
        private Dictionary<Type, UiControllerBase> m_UiControllers = new Dictionary<Type, UiControllerBase>();

        public TController GetUiController<TController>() where TController : UiControllerBase
        {
            m_UiControllers.TryGetValue(typeof(TController), out var uiController);
            return uiController as TController;
        }

        public UiControllerBase GetUiController(Type type)
        {
            m_UiControllers.TryGetValue(type, out var uiController);
            return uiController;
        }
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
