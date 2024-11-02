
namespace BbxCommon
{
    public abstract class TaskConditionBase : TaskBase
    {
        public enum EConditionState
        {
            Succeeded,
            Failed,
        }

        protected virtual void OnConditionEnter() { }
        protected abstract EConditionState OnConditionUpdate(float deltaTime);
        protected virtual void OnConditionExit() { }

        protected sealed override void OnEnter()
        {
            OnConditionEnter();
        }
        protected sealed override ETaskRunState OnUpdate(float deltaTime)
        {
            var state = OnConditionUpdate(deltaTime);
            switch (state)
            {
                case EConditionState.Succeeded:
                    return ETaskRunState.Succeeded;
                case EConditionState.Failed:
                    return ETaskRunState.Failed;
            }
            return ETaskRunState.Succeeded;
        }
        protected sealed override void OnExit()
        {
            OnConditionExit();
        }
    }
}
