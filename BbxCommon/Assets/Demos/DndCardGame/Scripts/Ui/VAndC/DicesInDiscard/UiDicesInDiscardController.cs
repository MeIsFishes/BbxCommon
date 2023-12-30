using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

namespace Dcg.Ui
{
    public class UiDicesInDiscardController : UiControllerBase<UiDicesInDiscardView>
    {
        private ObjRef<CombatDeckRawComponent> m_CombatDeckComp;
        private ModelListener m_DicesInDiscardRefreshListener;

        protected override void OnUiInit()
        {
            m_View.EventListener.OnPointerClick += OnClick;
            m_DicesInDiscardRefreshListener = ModelWrapper.CreateListener(EControllerLifeCycle.Show, CombatDeckRawComponent.EUiEvent.DicesInDiscardRefresh, OnDiscardRefresh);
        }

        protected override void OnUiOpen()
        {
            // 这里是直接从singleton拿的数据，属于玩家操控系统还没做出来的临时做法，后期应该需要优化
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var character = combatInfoComp.Character;
            m_CombatDeckComp = character.GetRawComponent<CombatDeckRawComponent>().AsObjRef();
            m_DicesInDiscardRefreshListener.RebindModelItem(m_CombatDeckComp.Obj);
            OnDiscardRefresh(m_CombatDeckComp.Obj);
        }

        protected override void OnUiDestroy()
        {
            m_View.EventListener.OnPointerClick -= OnClick;
        }

        private void OnClick(PointerEventData eventData)
        {
            if (m_CombatDeckComp.IsNull())
                return;
            UiApi.GetUiController<UiDiceDisplayController>().Display("抽牌堆", m_CombatDeckComp.Obj.DicesInDiscard);
        }

        private void OnDiscardRefresh(MessageData message)
        {
            var combatDeckComp = message.GetData<CombatDeckRawComponent>();
            OnDiscardRefresh(combatDeckComp);
        }

        private void OnDiscardRefresh(CombatDeckRawComponent combatDeckComp)
        {
            m_View.NumberText.text = combatDeckComp.DicesInDiscard.Count.ToString();
        }
    }
}
