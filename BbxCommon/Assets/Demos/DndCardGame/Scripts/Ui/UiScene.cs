using BbxCommon.Ui;
using UnityEngine;

namespace DCG
{
    public enum EUiSceneGroup
    {
        Info,
        SubMenu,
    }

    public class UiScene : UiSceneBase<EUiSceneGroup>
    {
        public void Awake()
        {
            CreateUiGroupRoot(EUiSceneGroup.Info);
            CreateUiGroupRoot(EUiSceneGroup.SubMenu);
        }
    }
}
