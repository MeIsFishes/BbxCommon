using BbxCommon;
using BbxCommon.Ui;
using System;

namespace Dcg.Ui
{
    public class UiDiceFrameView : UiViewBase
    {
        public UiInteractor UiInteractor;
        public GameObjectGroup DiceFilledGroup;

        public override Type GetControllerType()
        {
            return typeof(UiDiceFrameController);
        }

        public override string GetResourcePath()
        {
            return "DndCardGame/Prefabs/Ui/DiceFrame";
        }

        public override int GetUiGroup()
        {
            return (int)EUiSceneGroup.SubMenu;
        }
    }
}
