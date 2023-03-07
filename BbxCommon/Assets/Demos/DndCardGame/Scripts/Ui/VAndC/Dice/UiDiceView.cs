using BbxCommon.Ui;
using System;

namespace Dcg.Ui
{
    public class UiDiceView : UiViewBase
    {
        public UiDragable UiDragable;
        public UiInteractor UiInteractor;

        public override string GetResourcePath()
        {
            return "DndCardGame/Prefabs/Ui/Dice";
        }

        public override Type GetControllerType()
        {
            return typeof(UiDiceController);
        }

        public override int GetUiGroup()
        {
            return (int)EUiSceneGroup.SubMenu;
        }
    }
}
