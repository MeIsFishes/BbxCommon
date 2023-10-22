using UnityEngine;
using UnityEngine.EventSystems;
using BbxCommon;
using BbxCommon.Ui;
using UnityEngine.Events;

namespace Dcg.Ui
{
    /// <summary>
    /// 由于骰子卡牌需要在多处展示，为了复用代码，同时保证骰子被修改后可以在各个地方得到反应，其他地方（如手牌UI）
    /// 可以挂载一个空的UI对象，然后由这个UI对象来打开<see cref="UiDiceController"/>。因此<see cref="UiDiceController"/>
    /// 计划封装一些设置项，来满足不同业务对骰子的具体需求。
    /// </summary>
    public class UiDiceController : UiControllerBase<UiDiceView>
    {
        #region Interfaces
        public UnityAction<Interactor, Interactor> OnInteract
        {
            get
            {
                return m_View.UiInteractor.Wrapper.OnInteract;
            }
            set
            {
                m_View.UiInteractor.Wrapper.OnInteract = value;
            }
        }

        public void Bind(Dice dice)
        {
            m_View.DiceTittle.text = dice.GetDiceTittle();
        }

        public void SetUiDragableEnable(bool enable)
        {
            if (enable == true)
                Wrapper.EnableUiItem(m_View.UiDragable);
            else
                Wrapper.DisableUiItem(m_View.UiDragable);
        }

        public void SetUiInteractorEnable(bool enable)
        {
            if (enable == true)
                Wrapper.EnableUiItem(m_View.UiInteractor);
            else
                Wrapper.DisableUiItem(m_View.UiInteractor);
        }

        #region InitFunctions
        // 以下函数是用于各个地方（如手牌、卡组展示等）骰子初始化的函数，将它们写在这里而不是各自的controller下，
        // 是为了将来当骰子出现新的封装（如改变颜色、缩放）时，可以更方便地为不同情景设置默认值。
        public void InitDicesInHand()
        {
            SetUiDragableEnable(true);
            SetUiInteractorEnable(true);
        }
        #endregion
        #endregion

        #region Internal
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
        #endregion
    }
}
