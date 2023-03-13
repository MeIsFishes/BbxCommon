using UnityEngine;
using UnityEngine.EventSystems;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDiceController : UiControllerBase<UiDiceView>
    {
        protected override void OnUiInit()
        {
            base.OnUiInit();
            m_View.UiDragable.Wrapper.OnBeginDrag += OnBeginDrag;
            m_View.UiDragable.Wrapper.OnEndDrag += OnEndDrag;
            m_View.UiDragable.Wrapper.OnPointerEnter += OnPointerEnter;
            m_View.UiDragable.Wrapper.OnPointerExit += OnPointerExit;
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            m_View.OnDragGroup.Wrapper.SetActive();
            transform.localScale = new Vector3(m_View.OnDragScale, m_View.OnDragScale, m_View.OnDragScale);
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            m_View.OnDragGroup.Wrapper.SetInactive();
            transform.localScale = Vector3.one;
        }

        private void OnPointerEnter(PointerEventData eventData)
        {
            m_View.OnDragGroup.Wrapper.SetActive();
        }

        private void OnPointerExit(PointerEventData eventData)
        {
            m_View.OnDragGroup.Wrapper.SetInactive();
        }
    }
}
