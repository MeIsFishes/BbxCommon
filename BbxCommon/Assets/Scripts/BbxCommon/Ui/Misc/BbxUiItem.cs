
namespace BbxCommon.Ui
{
    public interface IUiPreInit
    {
        void OnUiPreInit(UiViewBase uiView);
    }

    public interface IUiInit
    {
        void OnUiInit(UiControllerBase uiController);
    }

    public interface IUiOpen
    {
        void OnUiOpen(UiControllerBase uiController);
    }

    public interface IUiShow
    {
        void OnUiShow(UiControllerBase uiController);
    }

    public interface IUiUpdate
    {
        void OnUiUpdate(UiControllerBase uiController, float deltaTime);
    }

    public interface IUiHide
    {
        void OnUiHide(UiControllerBase uiController);
    }

    public interface IUiClose
    {
        void OnUiClose(UiControllerBase uiController);
    }

    public interface IUiDestroy
    {
        void OnUiDestroy(UiControllerBase uiController);
    }
}
