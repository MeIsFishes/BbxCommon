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
    public class UiRewardDiceItemView : UiViewBase
    {
        public Transform DiceRoot;
        /// <summary>
        /// 会生成一个骰子专门用来播放动画
        /// </summary>
        public Transform TweenDiceRoot;
        public UiTweenGroup TweenGroup;

        public override Type GetControllerType()
        {
            return typeof(UiRewardDiceItemController);
        }
    }
}
