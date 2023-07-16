using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiTweenColor : UiTweenBase<Color>
    {
        [FoldoutGroup("Tween Targets")]
        public ESearchTarget SearchTarget;

        protected override void ApplyTween(Component component, float evaluate)
        {
            ((Graphic)component).color = MinValue + ((MaxValue - MinValue) * evaluate);
        }

        protected override ESearchTarget GetSearchTarget()
        {
            return SearchTarget;
        }

        protected override void GetSearchType(List<Type> types)
        {
            types.Add(typeof(Graphic));
        }
    }
}
