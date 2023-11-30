using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiCharacterInfoController : UiControllerBase<UiCharacterInfoView>
    {
        public void Bind(Entity entity)
        {
            var attributesComp = entity.GetRawComponent<AttributesRawComponent>();
            m_View.MaxHp.text = attributesComp.MaxHp.ToString();
            m_View.Strength.text = attributesComp.Strength.ToString();
            m_View.Dexterity.text = attributesComp.Dexterity.ToString();
            m_View.Constitution.text = attributesComp.Constitution.ToString();
            m_View.Intelligence.text = attributesComp.Intelligence.ToString();
            m_View.Wisdom.text = attributesComp.Wisdom.ToString();
            m_View.ArmorClass.text = GameUtility.DiceFunctions.ConvertDicesToString(attributesComp.ArmorClass);
        }
    }
}
