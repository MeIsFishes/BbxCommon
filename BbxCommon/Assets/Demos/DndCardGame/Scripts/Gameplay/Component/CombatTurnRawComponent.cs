using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public enum ECombatTurn
    {
        PlayerTurn,
        AllyTurn,
        EnemyTurn,
    }

    /// <summary>
    /// 战斗中的回合信息
    /// </summary>
    public class CombatTurnRawComponent : EcsRawComponent
    {
        /// <summary>
        /// 是否已经处在回合中
        /// </summary>
        public bool DuringTurn;
        public bool RequestBegin;
        public bool RequestEnd;
    }
}
