using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiTweenAlpha : UiTweenBase<float>
    {
        public enum ESearchType
        {
            CanvasGroup,
            Graphic,
        }

        [FoldoutGroup("Tween Targets")]
        public ESearchType SearchType;
        [FoldoutGroup("Tween Targets"), ShowIf("@SearchType == ESearchType.Graphic")]
        public ESearchTarget SearchTarget;

        protected override void ApplyTween(Component component, float evaluate)
        {
            if (component is CanvasGroup canvasGroup)
                canvasGroup.alpha = MinValue + ((MaxValue - MinValue) * evaluate);
            else if (component is Graphic graphic)
                graphic.color = graphic.color.SetA(MinValue + ((MaxValue - MinValue) * evaluate));
        }

        protected override ESearchTarget GetSearchTarget()
        {
            return SearchTarget;
        }

        protected override void GetSearchType(List<Type> types)
        {
            switch (SearchType)
            {
                case ESearchType.CanvasGroup:
                    types.Add(typeof(CanvasGroup));
                    break;
                case ESearchType.Graphic:
                    types.Add(typeof(Graphic));
                    break;
            }
        }

        protected override bool AllowAutoCreate()
        {
            if (SearchType == ESearchType.CanvasGroup)
                return true;
            return false;
        }
    }
}
