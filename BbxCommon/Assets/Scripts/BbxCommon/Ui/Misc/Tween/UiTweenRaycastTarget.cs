using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiTweenRaycastTarget : UiTweenBase
    {
        public enum ESearchType
        {
            CanvasGroup,
            Graphic,
        }

        [InfoBox("Set Graphic.raycastTarget or CanvasGroup.blockRaycast as true while the AnimationCurve evaluates 1 or greater, and as false while evaluates 0.01 or less.")]
        public ESearchType SearchType;

        protected override void ApplyTween(Component component, float evaluate)
        {
            switch (SearchType)
            {
                case ESearchType.CanvasGroup:
                    if (component is CanvasGroup canvasGroup)
                    {
                        if (evaluate > 0.99f)
                            canvasGroup.blocksRaycasts = true;
                        else if (evaluate < 0.01f)
                            canvasGroup.blocksRaycasts = false;
                    }
                    break;
                case ESearchType.Graphic:
                    if (component is Graphic graphic)
                    {
                        if (evaluate > 0.99f)
                            graphic.raycastTarget = true;
                        else if (evaluate < 0.01f)
                            graphic.raycastTarget = false;
                    }
                    break;
            }
        }

        protected override ESearchTarget GetSearchTarget()
        {
            switch (SearchType)
            {
                case ESearchType.CanvasGroup:
                    return ESearchTarget.Single;
                case ESearchType.Graphic:
                    return ESearchTarget.Multiple;
            }
            return ESearchTarget.Single;
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
