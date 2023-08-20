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
    public class HudMonsterStatusView : HudViewBase
    {
        public Image HpFill;
        public TMP_Text HpText;

        public override Type GetControllerType()
        {
            return typeof(HudMonsterStatusController);
        }
    }
}
