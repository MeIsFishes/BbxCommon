using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class AiBehaviourRawComponent : EcsRawComponent
    {
        /// <summary>
        /// 进行行动的延迟时间（回合开始时计时）
        /// </summary>
        public static float ActionDelay = 3.0f;
        /// <summary>
        /// 结束回合的延迟时间（回合开始时计时）
        /// </summary>
        public static float EndTurnDelay = 5.0f;
        public float ElapsedTime;
        public bool DidAction;
        public bool DidEndTurn;

        protected override void OnAllocate()
        {
            ElapsedTime = 0;
        }
    }
}
