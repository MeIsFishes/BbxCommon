using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;
using UnityEngine.EventSystems;

namespace Dcg.Ui
{
    public class UiDicesInDeckController : UiControllerBase<UiDicesInDeckView>
    {
        private ObjRef<CombatDeckRawComponent> m_CombatDeckComp;

        protected override void OnUiInit()
        {
            // 这里是直接从singleton拿的数据，属于玩家操控系统还没做出来的临时做法，后期应该需要优化
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var character = combatInfoComp.Character;
            m_CombatDeckComp = character.GetRawComponent<CombatDeckRawComponent>().AsObjRef();
            m_View.EventListener.OnPointerClick += OnClick;
            ModelWrapper.AddUiModelListener(EControllerLifeCycle.Show, m_CombatDeckComp.Obj, CombatDeckRawComponent.EUiEvent.DicesInDeckRefresh, OnDeckRefresh);
            OnDeckRefresh(m_CombatDeckComp.Obj);
        }

        protected override void OnUiDestroy()
        {
            m_View.EventListener.OnPointerClick -= OnClick;
        }

        private void OnClick(PointerEventData eventData)
        {
            if (m_CombatDeckComp.IsNull())
                return;
            UiApi.GetUiController<UiDiceDisplayController>().Display("抽牌堆", m_CombatDeckComp.Obj.DicesInDeck);
        }

        private void OnDeckRefresh(MessageDataBase message)
        {
            var combatDeckComp = message.GetData<CombatDeckRawComponent>();
            OnDeckRefresh(combatDeckComp);
        }

        private void OnDeckRefresh(CombatDeckRawComponent combatDeckComp)
        {
            m_View.NumberText.text = combatDeckComp.DicesInDeck.Count.ToString();
        }
    }
}
