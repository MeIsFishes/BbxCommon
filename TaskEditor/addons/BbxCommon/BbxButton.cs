using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
    public partial class BbxButton : Button
    {
        #region Common
        /// <summary>
        /// <para>Emitted when the button is toggled or pressed. This is on <see cref="Godot.BaseButton.ButtonDown"/> if <see cref="Godot.BaseButton.ActionMode"/> is <see cref="Godot.BaseButton.ActionModeEnum.Press"/> and on <see cref="Godot.BaseButton.ButtonUp"/> otherwise.</para>
        /// <para>If you need to know the button's pressed state (and <see cref="Godot.BaseButton.ToggleMode"/> is active), use <see cref="Godot.BaseButton.Toggled"/> instead.</para>
        /// </summary>
        public new Action Pressed;

        public override void _EnterTree()
        {
            base.Pressed += InvokePressed;
            VisibilityChanged += OnVisibilityChanged;
            OnVisibilityChanged();
        }

        public override void _ExitTree()
        {
            Visible = false;
            base.Pressed -= InvokePressed;
            VisibilityChanged -= OnVisibilityChanged;
        }

        private void InvokePressed()
        {
            if (Pressed != null)
            {
                Pressed();
            }
        }
        #endregion

        #region Hotkey
        public partial class CombinedHotkey : GodotObject
        {
            [Export]
            public Godot.Collections.Array<Key> Hotkeys;

            public CombinedHotkey(params Key[] keys)
            {
                Hotkeys = new Godot.Collections.Array<Key>(keys);
            }

            public CombinedHotkey(IEnumerable<Key> keys)
            {
                Hotkeys = new();
                Hotkeys.AddRange(keys);
            }

            public CombinedHotkey Clone()
            {
                var res = new CombinedHotkey();
                res.Hotkeys = new();
                res.Hotkeys.AddRange(Hotkeys);
                return res;
            }

            public bool Equals(CombinedHotkey other)
            {
                if (Hotkeys.Count != other.Hotkeys.Count)
                    return false;
                for (int i = 0; i < Hotkeys.Count; i++)
                {
                    if (other.Hotkeys.Contains(Hotkeys[i]) == false)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// The last registered callback Pressed will be invoked.
        /// For example, if a button registered hotkey Space, and the other one regisers then, the previous one will do nothing.
        /// </summary>
        [Export]
        public Godot.Collections.Array<Key> Hotkeys = new();
        /// <summary>
        /// If all keys in the Array are all pressed, the button Pressed will be invoked.
        /// </summary>
        [Export]
        public Godot.Collections.Array<Key> HotkeyGroup1 = new();
        /// <summary>
        /// If all keys in the Array are all pressed, the button Pressed will be invoked.
        /// </summary> 
        [Export]
        public Godot.Collections.Array<Key> HotkeyGroup2 = new();

        private void OnVisibilityChanged()
        {
            if (IsVisibleInTree())
                RegisterAllHotkey();
            else
                UnregisterAllHotkey();
        }

        private void RegisterAllHotkey()
        {
            for (int i = 0; i < Hotkeys.Count; i++)
            {
                var combinedHotkey = new CombinedHotkey(Hotkeys[i]);
                RegisterHotkey(combinedHotkey);
            }
            RegisterHotkey(new CombinedHotkey(HotkeyGroup1));
            RegisterHotkey(new CombinedHotkey(HotkeyGroup2));
        }

        private void UnregisterAllHotkey()
        {
            for (int i = 0; i < Hotkeys.Count; i++)
            {
                var combinedHotkey = new CombinedHotkey(Hotkeys[i]);
                UnregisterHotkey(combinedHotkey);
            }
            UnregisterHotkey(new CombinedHotkey(HotkeyGroup1));
            UnregisterHotkey(new CombinedHotkey(HotkeyGroup2));
        }

        private void RegisterHotkey(CombinedHotkey combinedHotkey)
        {
            if (combinedHotkey.Hotkeys.Count == 0)
                return;
            SimplePool.Alloc(out List<int> hotkeys);
            for (int i = 0; i < combinedHotkey.Hotkeys.Count; i++)
            {
                hotkeys.Add((int)combinedHotkey.Hotkeys[i]);
            }
            InputApi.RegisterHotkey(Pressed, InputApi.EHotkeyType.Mutex, hotkeys);
            hotkeys.CollectToPool();
        }

        private void UnregisterHotkey(CombinedHotkey combinedHotkey)
        {
            if (combinedHotkey.Hotkeys.Count == 0)
                return;
            SimplePool.Alloc(out List<int> hotkeys);
            for (int i = 0; i < combinedHotkey.Hotkeys.Count; i++)
            {
                hotkeys.Add((int)combinedHotkey.Hotkeys[i]);
            }
            InputApi.UnregisterHotkey(Pressed, InputApi.EHotkeyType.Mutex, hotkeys);
            hotkeys.CollectToPool();
        }
        #endregion
    }
}
