using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDiceController : UiControllerBase<UiDiceView>
    {
        protected override void OnUiInit()
        {
            base.OnUiInit();
            m_View.UiDragable.Init(this);
        }
    }
}
