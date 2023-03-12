using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDiceFrameController : UiControllerBase<UiDiceFrameView>
    {
        protected override void OnUiInit()
        {
            base.OnUiInit();
            m_View.UiInteractor.Wrapper.OnInteract += OnInteract;
            m_View.DiceFilledGroup.SetInactive();
        }

        private void OnInteract(Interactor requester, Interactor responser)
        {
            if (requester.HasFlag(EInteractorFlag.Dice))
            {
                m_View.DiceFilledGroup.SetActive();
            }
        }
    }
}
