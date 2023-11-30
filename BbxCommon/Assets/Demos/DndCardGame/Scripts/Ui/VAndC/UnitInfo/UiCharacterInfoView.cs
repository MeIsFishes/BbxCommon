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
    public class UiCharacterInfoView : UiViewBase
    {
        public TMP_Text MaxHp;
        public TMP_Text Strength;
        public TMP_Text Dexterity;
        public TMP_Text Constitution;
        public TMP_Text Intelligence;
        public TMP_Text Wisdom;
        public TMP_Text ArmorClass;

        public override Type GetControllerType()
        {
            return typeof(UiCharacterInfoController);
        }
    }
}
