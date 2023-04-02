using UnityEngine;

namespace BbxCommon
{
    public class TaskFlicker : TaskBase
    {
        public GameObject Target;
        public float FlickerInterval;
        public float Duration;

        private float m_PastTime;
        private float m_FadingTimer;
        private bool m_FadeIn;
        private SpriteRenderer m_SpriteComp;
        private Color m_OriginColor;
        private TaskFadeTo m_CurrentTask;

        public void Init(GameObject target, float flickerInterval, float duration)
        {
            Target = target;
            FlickerInterval = flickerInterval;
            Duration = duration;
            m_SpriteComp = Target.GetComponent<SpriteRenderer>();
            m_OriginColor = m_SpriteComp.color;
        }

        protected override void OnEnter()
        {
            m_PastTime = 0;
            m_FadingTimer = FlickerInterval;
            m_FadeIn = false;
        }

        protected override ETaskState OnExecute()
        {
            m_PastTime += Time.deltaTime;
            m_FadingTimer += Time.deltaTime;
            if (m_PastTime >= Duration)
            {
                return ETaskState.Finished;
            }
            if (m_FadingTimer >= FlickerInterval)
            {
                if (m_FadeIn)
                {
                    m_CurrentTask = TaskManager<TaskFadeTo>.Instance.CreateTask();
                    m_CurrentTask.Init(Target, 1, FlickerInterval);
                }
                else
                {
                    m_CurrentTask = TaskManager<TaskFadeTo>.Instance.CreateTask();
                    m_CurrentTask.Init(Target, 0.2f, FlickerInterval);
                }
                m_FadingTimer -= FlickerInterval;
                m_FadeIn = !m_FadeIn;
            }
            return ETaskState.Running;
        }

        protected override void OnExit()
        {
            m_CurrentTask.Exit();
            m_SpriteComp.color = new Color(
                m_OriginColor.r,
                m_OriginColor.g,
                m_OriginColor.b,
                1);
        }
    }
}
