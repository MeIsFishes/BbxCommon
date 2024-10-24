using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiTweenInteractable : UiTweenBase
    {
        [InfoBox("Set CanvasGroup.interactable as true while the AnimationCurve evaluates 0.99 or greater, and as false while evaluates 0.01 or less.")]
        [ReadOnly]
        public string Description;

        protected override void ApplyTween(Component component, float evaluate)
        {
            if (component is CanvasGroup canvasGroup)
            {
                if (evaluate > 0.99f)
                    canvasGroup.interactable = true;
                else if (evaluate < 0.01f)
                    canvasGroup.interactable = false;
            }
        }

        protected override ESearchTarget GetSearchTarget()
        {
            return ESearchTarget.Single;
        }

        protected override void GetSearchType(List<Type> types)
        {
            types.Add(typeof(CanvasGroup));
        }

        protected override bool AllowAutoCreate()
        {
            return true;
        }
    }
}
