using UnityEngine;

namespace BbxCommon.Ui
{
    /// <summary>
    /// <para>
    /// Sometimes there will be some UI items which are created by other items but not exist independently, so we
    /// need to remove them before pre-init, to avoid creating too many ones.
    /// </para><para>
    /// For example, <see cref="UiDragable"/> created a <see cref="UiTransformSetter"/> during its last pre-initialization.
    /// While modified, we have in fact hung it to another <see cref="GameObject"/>. In that case, the <see cref="UiTransformSetter"/>
    /// it created is an abandoned one, which need to be removed.
    /// </para>
    /// </summary>
    public interface IUiPreInitRemove
    {
        bool DontRemove { get; }
    }

    public interface IUiPreInit
    {
        /// <summary>
        /// If there is something need to be re-pre-init, return false, and then <see cref="UiViewBase.PreUiInit"/>
        /// will run an extra loop.
        /// </summary>
        bool OnUiPreInit(UiViewBase uiView);
    }

    public interface IUiInit
    {
        void OnUiInit(UiControllerBase uiController);
    }

    public interface IUiOpen
    {
        void OnUiOpen(UiControllerBase uiController);
    }

    public interface IUiShow
    {
        void OnUiShow(UiControllerBase uiController);
    }

    public interface IUiUpdate
    {
        void OnUiUpdate(UiControllerBase uiController, float deltaTime);
    }

    public interface IUiHide
    {
        void OnUiHide(UiControllerBase uiController);
    }

    public interface IUiClose
    {
        void OnUiClose(UiControllerBase uiController);
    }

    public interface IUiDestroy
    {
        void OnUiDestroy(UiControllerBase uiController);
    }
}
