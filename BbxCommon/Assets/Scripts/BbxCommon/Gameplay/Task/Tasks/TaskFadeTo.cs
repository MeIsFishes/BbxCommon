using UnityEngine;

namespace BbxCommon
{
    public class TaskFadeTo : TaskBase
    {
        public GameObject Target;
        public float FinalAlpha;    // 最终透明度
        public float Duration;

        private float m_FadeRate;   // 衰减速率
        private SpriteRenderer m_SpriteComp;
        private float m_BeginningTime;

        public void Init(GameObject target, float finalAlpha, float duration)
        {
            Target = target;
            FinalAlpha = finalAlpha;
            Duration = duration;
        }

        protected override void OnEnter()
        {
            Target.TryGetComponent(out m_SpriteComp);
            if (m_SpriteComp == null) return;
            m_FadeRate = (FinalAlpha - m_SpriteComp.color.a) / Duration;
            m_BeginningTime = Time.time;
        }

        protected override ETaskState OnExecute()
        {
            if (m_SpriteComp == null) return ETaskState.Finished;
            if (Time.time >= m_BeginningTime + Duration)
            {
                m_SpriteComp.color = new Color(
                m_SpriteComp.color.r,
                m_SpriteComp.color.g,
                m_SpriteComp.color.b,
                FinalAlpha);
                return ETaskState.Finished;
            }
            m_SpriteComp.color = new Color(
                m_SpriteComp.color.r,
                m_SpriteComp.color.g,
                m_SpriteComp.color.b,
                m_SpriteComp.color.a + m_FadeRate * Time.deltaTime);
            return ETaskState.Running;
        }

        protected override void OnExit()
        {
            
        }
    }
}
