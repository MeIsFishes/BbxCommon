using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiInteractorDicesInHandItemInfo : PooledObject
    {
        /// <summary>
        /// 该骰子在手牌中的序号
        /// </summary>
        public int Index;
        /// <summary>
        /// 对应CombatDeckRawComponent（存放手牌）的引用
        /// </summary>
        public ObjRef<CombatDeckRawComponent> CombatDeckComp;
    }

    public class UiDicesInHandItemController : UiControllerBase<UiDicesInHandItemView>
    {
        #region Operation
        // 把Operation写在每个UI内部，每个人就只需要维护自己的逻辑就行了
        public class OperationSwitchDiceInHandWithWild : OrderedOperationBase
        {
            public EntityID EntityID;
            public int DicesInHandIndex;
            public int WildDiceIndex;

            protected override void OnEnter()
            {
                if (EcsApi.GetEntityByID(EntityID, out var entity))
                {
                    var castSkillComp = entity.GetRawComponent<CastSkillRawComponent>();
                    var combatDeckComp = entity.GetRawComponent<CombatDeckRawComponent>();
                    // 记录自由骰位原本的骰子，以备假如要发生替换的情况
                    var originalWildDice = castSkillComp.WildDices[WildDiceIndex];
                    // 将自由骰的槽位替换成当前手牌
                    castSkillComp.WildDices[WildDiceIndex] = combatDeckComp.DicesInHand[DicesInHandIndex];
                    castSkillComp.DispatchEvent(CastSkillRawComponent.EUiEvent.WildDicesRefresh);
                    // 清除当前手牌，若原本自由骰槽位上已有骰子，则将它替换下来
                    combatDeckComp.DicesInHand.RemoveAt(DicesInHandIndex);
                    if (originalWildDice != null)
                        combatDeckComp.DicesInHand.Add(originalWildDice);
                    combatDeckComp.DispatchEvent(CombatDeckRawComponent.EUiEvent.DicesInHandRefresh);
                }
                
            }
        }
        #endregion

        private UiDiceController m_DiceController;
        private UiInteractorDicesInHandItemInfo m_InteractorInfo;

        protected override void OnUiInit()
        {
            m_DiceController = UiApi.OpenUiController<UiDiceController>(m_View.transform);
            m_DiceController.InitDicesInHand();
            m_DiceController.Show();
            m_DiceController.OnInteractWith += OnInteractWith;
        }

        public void Init(int index, CombatDeckRawComponent combatDeckComp)
        {
            m_InteractorInfo = ObjectPool<UiInteractorDicesInHandItemInfo>.Alloc();
            m_InteractorInfo.Index = index;
            m_InteractorInfo.CombatDeckComp = combatDeckComp.AsObjRef();
            m_DiceController.InteractorInfo = m_InteractorInfo;
        }

        public void Uninit()
        {
            m_InteractorInfo.CollectToPool();
            m_InteractorInfo = null;
            m_DiceController.InteractorInfo = null;
        }

        public void Bind(Dice dice)
        {
            m_DiceController.Bind(dice);
        }

        public void OnInteractWith(Interactor interactor)
        {
            // 由交互的发起方来处理全部的逻辑。
            // Q：为什么不让每方只处理自己的逻辑来完成解耦？
            // A：因为一方在处理完自己的逻辑后，比如自由骰位完成了替换，其值会发生变化，若再分开处理，会导致逻辑变得复杂。
            if (interactor.HasFlag(EInteractorFlag.WildDiceSlot))
            {
                var wildDiceInfo = interactor.ExtraInfo as UiInteractorWildDiceSlotInfo;
                if (m_InteractorInfo.CombatDeckComp.IsNull() || wildDiceInfo.CastSkillComp.IsNull())
                    return;
                var entity = m_InteractorInfo.CombatDeckComp.Obj.GetEntity();
                var operation = ObjectPool<OperationSwitchDiceInHandWithWild>.Alloc();
                operation.EntityID = entity.GetUniqueId();
                operation.DicesInHandIndex = m_InteractorInfo.Index;
                operation.WildDiceIndex = wildDiceInfo.Index;
                EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>().AddFreeOperation(operation);
            }
        }
    }
}
