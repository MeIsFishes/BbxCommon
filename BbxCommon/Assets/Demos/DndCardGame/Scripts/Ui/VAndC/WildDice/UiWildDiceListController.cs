using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiWildDiceListController : UiControllerBase<UiWildDiceListView>
    {
        #region Operation
        public class OperationRequestCastSkill : BlockedOperationBase
        {
            public Entity Entity;

            protected override void OnEnter()
            {
                var castSkillComp = Entity.GetRawComponent<CastSkillRawComponent>();
                castSkillComp.RequestCast = true;
                castSkillComp.Target = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>().Monster;
            }
        }
        #endregion

        private Entity Entity;
        private ObjRef<CastSkillRawComponent> m_CastSkillComp;
        private ListenableItemListener m_WildDicesRefreshListener;

        protected override void OnUiInit()
        {
            m_View.AcceptButton.onClick.AddListener(OnAcceptButton);
            m_WildDicesRefreshListener = ModelWrapper.CreateListener(EControllerLifeCycle.Open, (int)CastSkillRawComponent.EUiEvent.WildDicesRefresh, RefreshWildList);
        }

        protected override void OnUiClose()
        {
            for (int i = 0; i < m_View.SlotList.ItemWrapper.Count; i++)
            {
                m_View.SlotList.ItemWrapper.GetItem<UiWildDiceSlotController>(i).Uninit();
            }
        }

        public void Bind(Entity entity)
        {
            Entity = entity;
            m_CastSkillComp = entity.GetRawComponent<CastSkillRawComponent>().AsObjRef();
            m_WildDicesRefreshListener.RebindTarget(m_CastSkillComp.Obj);
            RefreshWildList(m_CastSkillComp.Obj);
        }

        private void RefreshWildList(MessageData message)
        {
            RefreshWildList(message.GetData<CastSkillRawComponent>());
        }

        /// <summary>
        /// 目前没有区分自由骰槽位变化和骰子变化，后面需求深入后可能会需要区分
        /// </summary>
        private void RefreshWildList(CastSkillRawComponent castSkillComp)
        {
            for (int i = 0; i < m_View.SlotList.ItemWrapper.Count; i++)
            {
                m_View.SlotList.ItemWrapper.GetItem<UiWildDiceSlotController>(i).Uninit();
            }
            m_View.SlotList.ItemWrapper.ModifyCount<UiWildDiceSlotController>(castSkillComp.WildDiceSlotCount);
            for (int i = 0; i < m_View.SlotList.ItemWrapper.Count; i++)
            {
                var slotController = m_View.SlotList.ItemWrapper.GetItem<UiWildDiceSlotController>(i);
                slotController.Init(i, castSkillComp);
                slotController.Bind(castSkillComp.WildDices[i]);
            }
        }

        /// <summary>
        /// 点击确定按钮，释放技能
        /// </summary>
        private void OnAcceptButton()
        {
            if (m_CastSkillComp.IsNull())
            {
                Debug.LogError("CastSkillRawComponent已被回收，疑似角色实体已销毁");
                return;
            }
            if (m_CastSkillComp.Obj.AllSlotFilled() == false)
            {
                UiApi.GetUiController<UiPromptController>().ShowPrompt("须填满自由骰后才可攻击！拖动手牌以填充自由骰槽位。");
                return;
            }

            var operation = ObjectPool<OperationRequestCastSkill>.Alloc();
            operation.Entity = m_CastSkillComp.Obj.GetEntity();
            EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>().AddBlockedOperation(operation);
            ControllerWrapper.Hide();
        }
    }
}