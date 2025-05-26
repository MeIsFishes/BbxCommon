using Godot;
using System;

namespace BbxCommon
{
    /// <summary>
    /// If you are writting a UI item, inherit this class.
    /// </summary>
	public partial class BbxControl : Control
	{
        public sealed override void _EnterTree()
        {
            base._EnterTree();
            OnUiInit();
            VisibilityChanged += () =>
            {
                if (Visible == true)
                    OnUiShow();
                else
                    OnUiHide();
            };
        }

        public sealed override void _Ready()
        {
            base._Ready();
            OnUiOpen();
        }

        public sealed override void _ExitTree()
        {
            base._ExitTree();
            OnUiClose();
            OnUiDestroy();
        }

        public sealed override void _Process(double delta)
        {
            base._Process(delta);
            OnUiUpdate(delta);
        }

        protected virtual void OnUiInit() { }
        protected virtual void OnUiOpen() { }
        protected virtual void OnUiShow() { }
        protected virtual void OnUiUpdate(double deltaTime) { }
        protected virtual void OnUiHide() { }
        protected virtual void OnUiClose() { }
        protected virtual void OnUiDestroy() { }
    }
}
