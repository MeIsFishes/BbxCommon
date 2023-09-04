using UnityEngine;
using UnityEngine.EventSystems;
using BbxCommon.Ui;
using System.Threading;

namespace Dcg.Ui
{
    public class UiDiceController : UiControllerBase<UiDiceView>
    {
        public void Bind(Dice dice)
        {
            m_View.DiceTittle.text = dice.GetDiceTittle();
        }

        protected override void OnUiInit()
        {
            base.OnUiInit();
            m_View.UiDragable.Wrapper.OnBeginDrag += OnBeginDrag;
            m_View.UiDragable.Wrapper.OnEndDrag += OnEndDrag;
            m_View.UiDragable.Wrapper.OnPointerEnter += OnPointerEnter;
            m_View.UiDragable.Wrapper.OnPointerExit += OnPointerExit;
        }

        protected override void OnUiShow()
        {
            m_View.OnDragGroup.SetInactive();
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            m_View.OnDragGroup.Wrapper.SetActive();
            m_View.UiDragable.transform.localScale = new Vector3(m_View.OnDragScale, m_View.OnDragScale, m_View.OnDragScale);
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            m_View.OnDragGroup.Wrapper.SetInactive();
            m_View.UiDragable.transform.localScale = Vector3.one;
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
