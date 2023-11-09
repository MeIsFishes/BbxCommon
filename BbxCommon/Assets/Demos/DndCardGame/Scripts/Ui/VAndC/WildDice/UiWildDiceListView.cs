using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiWildDiceListView : UiViewBase
    {
        public UiList SlotList;
        public Button AcceptButton;

        public override Type GetControllerType()
        {
            return typeof(UiWildDiceListController);
        }
    }
}
