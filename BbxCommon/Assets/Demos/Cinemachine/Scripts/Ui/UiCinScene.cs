using BbxCommon.Ui;

namespace Cin.Ui
{
    public enum EUiGroup
    {
        Info,
        Submenu,
        Normal,
    }

    public class UiCinScene : UiSceneBase<EUiGroup>
    {
        protected override void OnSceneInit()
        {
            UiGroupWrapper.CreateUiGroupRoot(EUiGroup.Info);
            UiGroupWrapper.CreateUiGroupRoot(EUiGroup.Submenu);
            UiGroupWrapper.CreateUiGroupRoot(EUiGroup.Normal);
        }
    }
}
