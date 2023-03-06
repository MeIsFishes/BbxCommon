using BbxCommon.Ui;

namespace Dcg.Ui
{
    public enum EUiSceneGroup
    {
        Info,
        SubMenu,
    }

    public class UiDungeonScene : UiSceneBase<EUiSceneGroup>
    {
        protected override void OnSceneInit()
        {
            CreateUiGroupRoot(EUiSceneGroup.Info);
            CreateUiGroupRoot(EUiSceneGroup.SubMenu);
        }
    }
}
