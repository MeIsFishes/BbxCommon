using UnityEngine;

namespace BbxCommon
{
    public class TaskMoveTo : TaskBase
    {
        public GameObject Target { get; private set; }
        public Vector3 TargetPosition { get; private set; }
        public float BeginningSpeed { get; private set; }
        public float Acceleration { get; private set; }

        private Vector3 m_Direction;
        private float m_Speed;

        public void Init(GameObject target, Vector3 targetPosition, float beginningSpeed, float acceleration = 0)
        {
            Target = target;
            TargetPosition = targetPosition;
            BeginningSpeed = beginningSpeed;
            Acceleration = acceleration;
        }

        protected override void OnEnter()
        {
            m_Direction = (TargetPosition - Target.transform.position).normalized;
            m_Speed = BeginningSpeed;
        }

        protected override ETaskState OnExecute()
        {
            var deltaDistance = m_Speed * Time.deltaTime;
            // 因为距离计算涉及到开方，性能会相对差一些，所以这里可以直接用四则运算和比较完成
            if (Mathf.Abs(TargetPosition.x - Target.transform.position.x) <= Mathf.Abs(m_Direction.x * deltaDistance) &&
                Mathf.Abs(TargetPosition.y - Target.transform.position.y) <= Mathf.Abs(m_Direction.y * deltaDistance))
            {
                Target.transform.position = TargetPosition;
                return ETaskState.Finished;
            }
            else
            {
                Target.transform.position += m_Direction * deltaDistance;
                m_Speed += Acceleration * Time.deltaTime;
                return ETaskState.Running;
            }
        }

        protected override void OnExit()
        {
            
        }
    }
}
