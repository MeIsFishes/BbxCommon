using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    public abstract class UiSceneBase : MonoBehaviour
    {
        #region Common
        public GameObject CanvasProto;

        public abstract void InitUiScene(GameObject canvasProto);
        public abstract void CreateUiByAsset(UiSceneAsset asset);
        public abstract void DestroyUiByAsset(UiSceneAsset asset);
        #endregion
    }

    public abstract class UiSceneBase<TGroupKey> : UiSceneBase where TGroupKey : Enum
    {
        #region Wrappers
        public UiControllerWpData UiControllerWrapper;
        public UiGroupWpData UiGroupWrapper;
        public UiModelWpData UiModelWrapper;

        public struct UiControllerWpData
        {
            private UiSceneBase<TGroupKey> m_Ref;

            public UiControllerWpData(UiSceneBase<TGroupKey> uiScene) { m_Ref = uiScene; }

            public void SetUiToGroup(GameObject uiGameObject, TGroupKey group) => m_Ref.SetUiToGroup(uiGameObject, group);
            public void SetUiToGroup(UiControllerBase uiController, TGroupKey group) => m_Ref.SetUiToGroup(uiController.gameObject, group);
        }

        public struct UiGroupWpData
        {
            private UiSceneBase<TGroupKey> m_Ref;

            public UiGroupWpData(UiSceneBase<TGroupKey> uiScene) { m_Ref = uiScene; }

            public Canvas CreateUiGroupRoot(TGroupKey uiGroup, string name = "") => m_Ref.CreateUiGroupRoot(uiGroup, name);
            public void SetUiGroup(List<TGroupKey> groups) => m_Ref.SetUiGroup(groups);
            public void SetUiGroup(params TGroupKey[] groups) => m_Ref.SetUiGroup(groups);
            public Canvas GetUiGroupCanvas(TGroupKey group) => m_Ref.GetUiGroupCanvas(group);
            public void SetUiToGroup(GameObject uiGameObject, TGroupKey group) => m_Ref.SetUiToGroup(uiGameObject, group);
            public void SetUiToGroup(UiControllerBase uiController, TGroupKey group) => m_Ref.SetUiToGroup(uiController.gameObject, group);
        }

        public struct UiModelWpData
        {
            private UiSceneBase<TGroupKey> m_Ref;

            public UiModelWpData(UiSceneBase<TGroupKey> uiScene) { m_Ref = uiScene; }

            public void AddUiModel<T>(T model) where T : UiModelBase => m_Ref.AddUiModel(model);
            public void TryGetUiModel<T>(out T model) where T : UiModelBase => m_Ref.TryGetUiModel(out model);
            public T TryGetUiModel<T>() where T : UiModelBase => m_Ref.TryGetUiModel<T>();
        }
        #endregion

        #region Common
        public sealed override void InitUiScene(GameObject canvasProto)
        {
            UiControllerWrapper = new UiControllerWpData(this);
            UiGroupWrapper = new UiGroupWpData(this);
            UiModelWrapper = new UiModelWpData(this);

            CanvasProto = canvasProto;
            OnSceneInit();
        }

        protected virtual void OnSceneInit() { }

        public override void CreateUiByAsset(UiSceneAsset asset)
        {
            if (asset == null)
                return;
            foreach (var data in asset.UiObjectDatas)
            {
                // if this is the first time of opening
                if (data.PrefabGameObject == null)
                {
                    data.PrefabGameObject = Resources.Load<GameObject>(data.PrefabPath);
                    data.UiView = data.PrefabGameObject.GetComponent<UiViewBase>();
                    data.ControllerType = data.UiView.GetControllerType();
                    data.ControllerTypeId = UiApi.GetUiControllerTypeId(data.UiView);
                }    
                var controller = UiApi.OpenUiController(data.UiView, data.ControllerTypeId, m_UiGroups[(TGroupKey)(object)data.UiGroup].transform);
                data.CreatedController = controller;
                var viewTransform = controller.View.transform as RectTransform;
                viewTransform.localPosition = data.Position;
                viewTransform.localScale = data.Scale;
                viewTransform.pivot = data.Pivot;
                if (data.DefaultShow)   // keep OnUiShow() calls after setting data
                    controller.Show();
            }
        }

        public override void DestroyUiByAsset(UiSceneAsset asset)
        {
            if (asset == null)
                return;
            foreach (var data in asset.UiObjectDatas)
            {
                data.CreatedController.Close();
            }
        }
        #endregion

        #region UiGroup
        protected Dictionary<TGroupKey, Canvas> m_UiGroups = new Dictionary<TGroupKey, Canvas>();

        public Canvas CreateUiGroupRoot(TGroupKey uiGroup, string name = "")
        {
            var root = Instantiate(CanvasProto);
            if (name.IsNullOrEmpty())
                root.name = uiGroup.ToString();
            else
                root.name = name;
            root.transform.SetParent(this.transform);
            var rootCanvas = root.GetComponent<Canvas>();
            m_UiGroups[uiGroup] = rootCanvas;
            return rootCanvas;
        }

        public void SetUiGroup(List<TGroupKey> groups)
        {
            foreach (var pair in m_UiGroups)
            {
                // set Canvas enable instead of setting GameObject to avoid destroying batched and rendered data
                if (groups.Contains(pair.Key))
                    pair.Value.enabled = true;
                else
                    pair.Value.enabled = false;
            }
        }

        public void SetUiGroup(params TGroupKey[] groups)
        {
            var list = SimplePool<List<TGroupKey>>.Alloc();
            list.AddArray(groups);
            SetUiGroup(list);
            list.CollectToPool();
        }

        public Canvas GetUiGroupCanvas(TGroupKey group)
        {
            return m_UiGroups[group];
        }

        public void SetUiToGroup(GameObject uiGameObject, TGroupKey group)
        {
            var uiGroupRoot = GetUiGroupCanvas(group).gameObject.transform;
            var originalActive = uiGameObject.gameObject.activeSelf;
            uiGameObject.gameObject.SetActive(false);
            uiGameObject.transform.SetParent(uiGroupRoot);
            uiGameObject.gameObject.SetActive(originalActive);
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
