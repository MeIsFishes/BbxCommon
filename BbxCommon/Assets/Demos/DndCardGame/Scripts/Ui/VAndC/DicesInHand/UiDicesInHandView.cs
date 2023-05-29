using BbxCommon.Ui;
using System;

namespace Dcg.Ui
{
    public class UiDicesInHandView : UiViewBase
    {
        public UiList DicesList;

        public override Type GetControllerType()
        {
            return typeof(UiDicesInHandController);
        }

        public override string GetResourcePath()
        {
            return "DndCardGame/Prefabs/Ui/DicesInHand";
        }

        public override int GetUiGroup()
        {
            return (int)EUiSceneGroup.Normal;
        }
    }
}
