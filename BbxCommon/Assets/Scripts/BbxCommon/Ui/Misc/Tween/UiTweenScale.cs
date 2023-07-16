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
            m_OriginalScale = transform.localScale;
        }

        protected override void ApplyTween(Component component, float evaluate)
        {
            switch (ScaleType)
            {
                case EScaleType.RelativeScale:
                    ((Transform)component).localScale = Vector3.Scale(m_OriginalScale, MinValue + (MaxValue - MinValue) * evaluate);
                    break;
                case EScaleType.AbsoluteScale:
                    ((Transform)component).localScale = MinValue + (MaxValue - MinValue) * evaluate;
                    break;
            }
        }

        protected override ESearchTarget GetSearchTarget()
        {
            return ESearchTarget.Single;
        }

        protected override void GetSearchType(List<Type> types)
        {
            types.Add(typeof(Transform));
        }
    }
}
