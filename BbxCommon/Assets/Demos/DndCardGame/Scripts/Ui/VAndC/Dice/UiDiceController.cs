using UnityEngine;
using UnityEngine.EventSystems;
using BbxCommon;
using BbxCommon.Ui;
using UnityEngine.Events;

namespace Dcg.Ui
{
    /// <summary>
    /// �������ӿ�����Ҫ�ڶദչʾ��Ϊ�˸��ô��룬ͬʱ��֤���ӱ��޸ĺ�����ڸ����ط��õ���Ӧ�������ط���������UI��
    /// ���Թ���һ���յ�UI����Ȼ�������UI��������<see cref="UiDiceController"/>�����<see cref="UiDiceController"/>
    /// �ƻ���װһЩ����������㲻ͬҵ������ӵľ�������
    /// </summary>
    public class UiDiceController : UiControllerBase<UiDiceView>
    {
        #region Interfaces
        public UnityAction<Interactor, Interactor> OnInteract
        {
            get { return m_View.UiInteractor.Wrapper.OnInteract; }
            set { m_View.UiInteractor.Wrapper.OnInteract = value; }
        }

        public UnityAction<Interactor> OnInteractWith
        {
            get { return m_View.UiInteractor.Wrapper.OnInteractWith; }
            set { m_View.UiInteractor.Wrapper.OnInteractWith = value; }
        }

        public object InteractorInfo
        {
            get { return m_View.UiInteractor.Wrapper.ExtraInfo; }
            set { m_View.UiInteractor.Wrapper.ExtraInfo = value; }
        }

        public void Bind(Dice dice)
        {
            m_View.DiceTittle.text = dice.GetDiceTittle();
        }

        public void SetUiDragableEnable(bool enable)
        {
            if (enable == true)
                ControllerWrapper.EnableUiItem(m_View.UiDragable);
            else
                ControllerWrapper.DisableUiItem(m_View.UiDragable);
        }

        public void SetUiInteractorEnable(bool enable)
        {
            if (enable == true)
                ControllerWrapper.EnableUiItem(m_View.UiInteractor);
            else
                ControllerWrapper.DisableUiItem(m_View.UiInteractor);
        }

        public void SetScale(float scale)
        {
            m_View.transform.localScale = new Vector3(scale, scale, 0);
        }

        #region InitFunctions
        // ���º��������ڸ����ط��������ơ�����չʾ�ȣ����ӳ�ʼ���ĺ�����������д����������Ǹ��Ե�controller�£�
        // ��Ϊ�˽��������ӳ����µķ�װ����ı���ɫ�����ţ�ʱ�����Ը������Ϊ��ͬ�龰����Ĭ��ֵ��
        public void InitDicesInHand()
        {
            SetUiDragableEnable(true);
            SetUiInteractorEnable(true);
        }

        public void InitWildDiceSlot(float scale)
        {
            SetUiDragableEnable(false);
            SetUiInteractorEnable(false);
            SetScale(scale);
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
