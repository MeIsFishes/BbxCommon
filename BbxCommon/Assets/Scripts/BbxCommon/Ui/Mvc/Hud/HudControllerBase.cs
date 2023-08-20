using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace BbxCommon.Ui
{
    // The whole life cycle of HudController is:
    // Init -> Open -> Bind -> Show -> Hide -> Unbind -> Close -> Destroy
    public interface IHudController
    {
        void Bind(Entity entity);
        void Unbind();
        void Show();
        void Hide();
        void Close();
        void Destroy();
        string GetResourcePath();
    }

    public abstract class HudControllerBase<TView> : UiControllerBase<TView>, IHudController where TView : HudViewBase
    {
        #region Common
        public Entity Entity = Entity.Null;

        private RectTransform m_ViewTransform;

        public void Bind(Entity entity)
        {
            if (entity == Entity.Null || Entity == entity)
                return;
            if (Entity != null)
                Unbind();
            Entity = entity;
            OnHudBind(entity);
        }

        public void Unbind()
        {
            if (Entity != null)
                OnHudUnbind();
        }
        #endregion

        #region LifeCycle
        protected override sealed void OnUiInit()
        {
            m_ViewTransform = m_View.gameObject.AddMissingComponent<RectTransform>();
            OnHudInit();
        }

        protected override sealed void OnUiOpen()
        {
            OnHudOpen();
        }

        protected override sealed void OnUiShow()
        {
            OnHudShow();
        }

        protected override void OnUiUpdate(float deltaTime)
        {
            if (m_View.AutoUpdatePos)
                UpdatePos();
            OnHudUpdate(deltaTime);
        }

        protected override sealed void OnUiHide()
        {
            OnHudHide();
        }

        protected override sealed void OnUiClose()
        {
            Unbind();
            OnHudClose();
        }

        protected override sealed void OnUiDestroy()
        {
            OnHudDestroy();
        }

        protected virtual void OnHudInit() { }
        protected virtual void OnHudOpen() { }
        protected virtual void OnHudShow() { }
        protected virtual void OnHudUpdate(float deltaTime) { }
        protected virtual void OnHudHide() { }
        protected virtual void OnHudClose() { }
        protected virtual void OnHudDestroy() { }
        protected virtual void OnHudBind(Entity entity) { }
        protected virtual void OnHudUnbind() { }
        #endregion

        #region UpdateTransform
        private void UpdatePos()
        {
            var entityGo = Entity.GetGameObject();
            if (entityGo == null)
                return;
            m_ViewTransform.localPosition = Camera.main.WorldToViewportPoint(entityGo.transform.position + m_View.HudOffset);
            RectTransform canvasTransform = UiApi.HudRoot.GetComponent<RectTransform>();
            m_ViewTransform.localPosition = new Vector3(m_ViewTransform.localPosition.x * canvasTransform.sizeDelta.x,
                m_ViewTransform.localPosition.y * canvasTransform.sizeDelta.y);
        }
        #endregion

        #region Abstract
        /// <summary>
        /// Return the path of current HUD prefab under the Resources folder, without "Resources/".
        /// </summary>
        public abstract string GetResourcePath();
        #endregion
    }
}
