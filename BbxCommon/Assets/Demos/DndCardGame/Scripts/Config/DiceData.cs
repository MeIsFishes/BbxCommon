using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;

namespace Dcg
{
    /// <summary>
    /// 设置奖励界面骰子出现的概率
    /// </summary>
    [CreateAssetMenu(fileName = "DiceData", menuName = "Demos/Dcg/DiceData")]
    public class DiceData : BbxScriptableObject
    {
        [FoldoutGroup("Reward"), LabelText("d4Weight")]
        public int RewardD4Weight;
        [FoldoutGroup("Reward"), LabelText("d6Weight")]
        public int RewardD6Weight;
        [FoldoutGroup("Reward"), LabelText("d8Weight")]
        public int RewardD8Weight;
        [FoldoutGroup("Reward"), LabelText("d10Weight")]
        public int RewardD10Weight;
        [FoldoutGroup("Reward"), LabelText("d12Weight")]
        public int RewardD12Weight;
        [FoldoutGroup("Reward"), LabelText("d20Weight")]
        public int RewardD20Weight;

        protected override void OnLoad()
        {
            DataApi.SetData(this);
        }
    }
}
