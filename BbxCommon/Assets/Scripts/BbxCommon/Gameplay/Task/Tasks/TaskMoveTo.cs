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
            if ((TargetPosition - Target.transform.position).magnitude < deltaDistance)
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
