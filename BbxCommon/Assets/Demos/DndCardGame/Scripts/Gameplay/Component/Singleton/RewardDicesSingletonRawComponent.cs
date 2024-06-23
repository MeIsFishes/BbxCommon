using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    /// <summary>
    /// 存储奖励页面的骰子
    /// </summary>
    public class RewardDicesSingletonRawComponent : EcsSingletonRawComponent, IListenable
    {
        public enum EUiEvent
        {
            DicesRefresh,
        }

        public List<Dice> Dices = new();
        public bool Chosen;

        private MessageHandler<int> m_MessageHandler = new();
        IMessageDispatcher<int> IListenable.MessageDispatcher => m_MessageHandler;

        public void DispatchEvent(EUiEvent uiEvent)
        {
            m_MessageHandler.Dispatch(uiEvent.GetHashCode(), this);
        }

        public override void OnCollect()
        {
            Dices.CollectToPool();
            Chosen = false;
        }
    }
}
