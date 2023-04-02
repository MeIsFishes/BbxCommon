using UnityEngine;

namespace BbxCommon
{
    public class TaskWaitAndDo : TaskBase
    {
        public delegate void DoFunctionDelegate();

        public float WaitingTime;
        public DoFunctionDelegate DoFunction;

        public void Init(float waitingTime, DoFunctionDelegate doFunction)
        {
            WaitingTime = waitingTime;
            DoFunction = doFunction;
        }

        protected override void OnEnter()
        {
            
        }

        protected override ETaskState OnExecute()
        {
            WaitingTime -= Time.deltaTime;
            if (WaitingTime <= 0)
            {
                return ETaskState.Finished;
            }
            else
            {
                return ETaskState.Running;
            }
        }

        protected override void OnExit()
        {
            DoFunction();
        }
    }
}
