﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiAttackView : UiViewBase
    {
        public Button AttackButton;
        public UiOptional WeaponOptions;
        public TMP_Text Description;

        public override Type GetControllerType()
        {
            return typeof(UiAttackController);
        }
    }
}
