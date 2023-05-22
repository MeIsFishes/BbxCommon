using BbxCommon.Ui;

namespace Dcg.Ui
{
    public enum EUiSceneGroup
    {
        Info,
        SubMenu,
        Normal,
    }

    public class UiDungeonScene : UiSceneBase<EUiSceneGroup>
    {
        protected override void OnSceneInit()
        {
            UiGroupWrapper.CreateUiGroupRoot(EUiSceneGroup.Info);
            UiGroupWrapper.CreateUiGroupRoot(EUiSceneGroup.SubMenu);
            UiGroupWrapper.CreateUiGroupRoot(EUiSceneGroup.Normal);
        }
    }
}
