using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiInteractorWildDiceSlotInfo : PooledObject
    {
        /// <summary>
        /// 该槽位在自由骰中的序号
        /// </summary>
        public int Index;
        /// <summary>
        /// 对应CastSkillRawComponent（存放自由骰）的引用
        /// </summary>
        public ObjRef<CastSkillRawComponent> CastSkillComp;
    }

    /// <summary>
    /// 自由骰的一个槽位，负责把拖拽到其上的骰子记录到自由骰的component中，并在界面上显示出来
    /// </summary>
    public class UiWildDiceSlotController : UiControllerBase<UiWildDiceSlotView>
    {
        private UiDiceController m_DiceController;
        private UiInteractorWildDiceSlotInfo m_InteractorInfo;

        public void Init(int index, CastSkillRawComponent castSkillComp)
        {
            m_InteractorInfo = ObjectPool<UiInteractorWildDiceSlotInfo>.Alloc();
            m_InteractorInfo.Index = index;
            m_InteractorInfo.CastSkillComp = castSkillComp.AsObjRef();
            m_View.UiInteractor.ExtraInfo = m_InteractorInfo;
        }

        public void Uninit()
        {
            m_InteractorInfo.CollectToPool();
            m_InteractorInfo = null;
            m_View.UiInteractor.ExtraInfo = null;
        }

        public void Bind(Dice dice)
        {
            if (m_DiceController == null)
            {
                m_DiceController = UiApi.OpenUiController<UiDiceController>(m_View.DiceRoot);
            }

            if (dice != null)
            {
                m_DiceController.Bind(dice);
                m_DiceController.InitWildDiceSlot(m_View.DiceScale);
                m_DiceController.Show();
            }
            else
            {
                m_DiceController.Hide();
            }
        }
    }
}
