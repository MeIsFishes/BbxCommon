using BbxCommon.Ui;
using UnityEngine;

namespace BbxCommon
{
 
    public abstract class UiLoadingControllerBase<T> : UiControllerBase<T> where T : UiViewBase
    {
        public abstract void OnLoading(float process);

        protected override void OnUiUpdate(float deltaTime)
        {
            OnLoading(GameEngineFacade.LoadingProgress);
            OnLoadingUpdate(deltaTime);
        }

        protected virtual void OnLoadingUpdate(float deltaTime) { }
    }
}