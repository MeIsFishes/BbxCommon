using BbxCommon.Ui;

namespace Dcg.Ui
{
    public enum EUiGroup
    {
        Info,
        SubMenu,
        UnderNormal,
        Normal,
    }

    public class UiDungeonScene : UiSceneBase<EUiGroup>
    {
        protected override void OnSceneInit()
        {
            UiGroupWrapper.CreateUiGroupRoot(EUiGroup.Info);
            UiGroupWrapper.CreateUiGroupRoot(EUiGroup.SubMenu);
            UiGroupWrapper.CreateUiGroupRoot(EUiGroup.UnderNormal);
            UiGroupWrapper.CreateUiGroupRoot(EUiGroup.Normal);
        }
    }
}
