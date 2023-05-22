using BbxCommon.Ui;
using System;

namespace Dcg.Ui
{
    public class DicesInHandView : UiViewBase
    {
        public UiList DicesList;

        public override Type GetControllerType()
        {
            return typeof(DicesInHandController);
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
