using System;
using UnityEngine;
using TMPro;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDiceView : UiViewBase
    {
        public TMP_Text DiceTittle;
        public UiDragable UiDragable;
        public UiInteractor UiInteractor;
        [Tooltip("GameObjects in the group will be shown when dragging, and be hidden otherwise.")]
        public GameObjectGroup OnDragGroup;
        [Tooltip("The value of local scale of the whole UI item when dragging.")]
        public float OnDragScale;

        public override Type GetControllerType()
        {
            return typeof(UiDiceController);
        }
    }
}
