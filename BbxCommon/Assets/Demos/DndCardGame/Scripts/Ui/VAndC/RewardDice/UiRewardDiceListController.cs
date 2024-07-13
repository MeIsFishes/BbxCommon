using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiRewardDiceListController : UiControllerBase<UiRewardDiceListView>
    {
        private ListenableItemListener m_ModelListener;

        protected override void OnUiInit()
        {
            m_ModelListener = ModelWrapper.CreateListener(EControllerLifeCycle.Show, RewardDicesSingletonRawComponent.EUiEvent.DicesRefresh, OnDicesRefresh);
            m_View.CompleteButton.onClick.AddListener(() => { DcgGameEngine.Instance.ChooseRewardComplete(); });
        }

        protected override void OnUiOpen()
        {
            var rewardDiceComp = EcsApi.GetSingletonRawComponent<RewardDicesSingletonRawComponent>();
            m_ModelListener.RebindTarget(rewardDiceComp);
            OnDicesRefresh(rewardDiceComp);
        }

        private void OnDicesRefresh(MessageData messageData)
        {
            var rewardDiceComp = messageData.GetData<RewardDicesSingletonRawComponent>();
            if (rewardDiceComp != null)
                OnDicesRefresh(rewardDiceComp);
        }

        private void OnDicesRefresh(RewardDicesSingletonRawComponent rewardDicesComp)
        {
            // 找到当前玩家的DeckComp
            var localPlayerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
            var deckComp = localPlayerComp.DungeonEntities[0].GetRawComponent<CharacterDeckRawComponent>();
            // 重置UiList
            m_View.UiList.ItemWrapper.ModifyCount<UiRewardDiceItemController>(rewardDicesComp.Dices.Count);
            for (int i = 0; i < rewardDicesComp.Dices.Count; i++)
            {
                var itemController = m_View.UiList.ItemWrapper.GetItem<UiRewardDiceItemController>(i);
                itemController.Bind(rewardDicesComp.Dices[i]);
                itemController.Init(deckComp, i);
            }
        }
    }
}
