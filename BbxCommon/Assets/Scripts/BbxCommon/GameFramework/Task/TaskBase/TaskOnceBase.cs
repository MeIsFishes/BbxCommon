using System.Collections.Generic;

namespace BbxCommon
{
    public abstract class TaskOnceBase : TaskBase
    {
        protected enum EOnceState
        {
            Succeeded,
            Failed,
        }

        protected sealed override void OnEnter() { }
        protected sealed override ETaskRunState OnUpdate(float deltaTime)
        {
            switch (OnExecute())
            {
                case EOnceState.Succeeded:
                    return ETaskRunState.Succeeded;
                case EOnceState.Failed:
                    return ETaskRunState.Failed;
            }
            return ETaskRunState.Succeeded;
        }
        protected sealed override void OnExit() { }

        protected abstract EOnceState OnExecute();
    }
}
