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
    public class UiTipListView : UiViewBase
    {
        public UiList UiList;

        public override Type GetControllerType()
        {
            return typeof(UiTipListController);
        }
    }
}
