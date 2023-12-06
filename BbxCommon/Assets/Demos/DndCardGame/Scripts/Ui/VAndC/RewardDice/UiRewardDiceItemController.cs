using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiRewardDiceItemController : UiControllerBase<UiRewardDiceItemView>
    {
        #region Operation
        public class OperationChooseRewardDice : FreeOperationBase
        {
            public Entity Entity;
            public int IndexOfReward;

            protected override void OnEnter()
            {
                var deckComp = Entity.GetRawComponent<CharacterDeckRawComponent>();
                var rewardDicesComp = EcsApi.GetSingletonRawComponent<RewardDicesSingletonRawComponent>();
                if (rewardDicesComp == null || rewardDicesComp.Chosen == true)
                    return;
                deckComp.Dices.Add(rewardDicesComp.Dices[IndexOfReward]);
                deckComp.DispatchEvent(CharacterDeckRawComponent.EUiEvent.DeckRefresh);
                rewardDicesComp.Dices[IndexOfReward] = null;
                rewardDicesComp.Chosen = true;
                rewardDicesComp.DispatchEvent(RewardDicesSingletonRawComponent.EUiEvent.DicesRefresh);
            }
        }
        #endregion

        private ObjRef<CharacterDeckRawComponent> m_DeckComp;
        private int m_IndexOfReward;
        private UiDiceController m_DiceController;
        private UiDiceController m_TweenDiceController;

        public void Init(CharacterDeckRawComponent deckComp, int index)
        {
            m_DeckComp = deckComp.AsObjRef();
            m_IndexOfReward = index;
        }

        public void Bind(Dice dice)
        {
            if (dice != null)
            {
                m_DiceController.Bind(dice);
                m_DiceController.Show();
                m_TweenDiceController.Bind(dice);
            }
            else
            {
                m_DiceController.Hide();
            }
        }

        protected override void OnUiInit()
        {
            m_DiceController = UiApi.OpenUiController<UiDiceController>(m_View.DiceRoot);
            m_DiceController.InitDisplay(1);
            m_TweenDiceController = UiApi.OpenUiController<UiDiceController>(m_View.TweenDiceRoot);
            m_DiceController.InitDisplay(1);
            // 获得骰子的动画，播放完成后隐藏骰子
            m_View.TweenGroup.Wrapper.OnPlayingFinishes += () => { m_TweenDiceController.Hide(); };
            m_DiceController.OnClick += OnChosen;
        }

        private void OnChosen(PointerEventData eventData)
        {
            if (m_DeckComp.IsNull())
                return;
            var operation = ObjectPool<OperationChooseRewardDice>.Alloc();
            operation.Entity = m_DeckComp.Obj.GetEntity();
            operation.IndexOfReward = m_IndexOfReward;
            EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>().AddFreeOperation(operation);
            m_TweenDiceController.Show();
            m_View.TweenGroup.Play();
        }
    }
}
