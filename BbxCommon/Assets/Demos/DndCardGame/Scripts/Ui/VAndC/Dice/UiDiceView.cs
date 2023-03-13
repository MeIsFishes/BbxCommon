using System;
using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDiceView : UiViewBase
    {
        public UiDragable UiDragable;
        public UiInteractor UiInteractor;
        [Tooltip("GameObjects in the group will be shown when dragging, and be hidden otherwise.")]
        public GameObjectGroup OnDragGroup;
        [Tooltip("The value of local scale of the whole UI item when dragging.")]
        public float OnDragScale;

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
