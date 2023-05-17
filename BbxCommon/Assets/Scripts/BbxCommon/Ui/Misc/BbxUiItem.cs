
namespace BbxCommon.Ui
{
    internal interface IBbxUiItem
    {
        void PreInit(UiViewBase uiView);
        void OnUiInit(UiControllerBase uiController);
        void OnUiOpen(UiControllerBase uiController);
        void OnUiShow(UiControllerBase uiController);
        void OnUiUpdate(UiControllerBase uiController);
        void OnUiHide(UiControllerBase uiController);
        void OnUiClose(UiControllerBase uiController);
        void OnUiDestroy(UiControllerBase uiController);
    }
}
