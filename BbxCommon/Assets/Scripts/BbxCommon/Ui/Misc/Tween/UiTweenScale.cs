using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiTweenScale : UiTweenBase<Vector3>
    {
        public enum EScaleType
        {
            RelativeScale,
            AbsoluteScale,
        }

        [FoldoutGroup("Play Tween")]
        public EScaleType ScaleType;

        private Vector3 m_OriginalScale;

        protected override void OnTweenShow()
        {
            m_OriginalScale = TransformRootOverride.localScale;
        }

        protected override void ApplyTween(Component component, float evaluate)
        {
            switch (ScaleType)
            {
                case EScaleType.RelativeScale:
                    ((UiTransformSetter)component).ScaleWrapper.AddScaleRequest(Vector3.Scale(m_OriginalScale, MinValue + (MaxValue - MinValue) * evaluate), UiTransformSetter.EScalePriority.Tween);
                    break;
                case EScaleType.AbsoluteScale:
                    ((UiTransformSetter)component).ScaleWrapper.AddScaleRequest(MinValue + (MaxValue - MinValue) * evaluate, UiTransformSetter.EScalePriority.Tween);
                    break;
            }
        }

        protected override ESearchTarget GetSearchTarget()
        {
            return ESearchTarget.Single;
        }

        protected override void GetSearchType(List<Type> types)
        {
            types.Add(typeof(UiTransformSetter));
        }

        protected override bool AllowAutoCreate()
        {
            return true;
        }
    }
}
