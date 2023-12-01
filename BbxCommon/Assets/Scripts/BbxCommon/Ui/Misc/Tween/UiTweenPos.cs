using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiTweenPos : UiTweenBase<Vector3>
    {
        public enum EPosType
        {
            RelativeLocalPos,
            AbsoluteLocalPos,
        }

        [FoldoutGroup("Play Tween")]
        public EPosType PosType = EPosType.AbsoluteLocalPos;

        private Vector3 m_OriginalPos;

        protected override void OnTweenShow()
        {
            m_OriginalPos = TransformRootOverride.localPosition;
        }

        protected override void ApplyTween(Component component, float evaluate)
        {
            switch (PosType)
            {
                case EPosType.RelativeLocalPos:
                    ((UiTransformSetter)component).PosWrapper.AddLocalPositionRequest(m_OriginalPos + MinValue + (MaxValue - MinValue) * evaluate, UiTransformSetter.EPosPriority.Tween);
                    break;
                case EPosType.AbsoluteLocalPos:
                    ((UiTransformSetter)component).PosWrapper.AddLocalPositionRequest(MinValue + (MaxValue - MinValue) * evaluate, UiTransformSetter.EPosPriority.Tween);
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
