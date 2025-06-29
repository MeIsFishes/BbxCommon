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

        private static Dictionary<CombinedHotkey, List<BbxButton>> m_RegisteredButton = new();
        private static HashSet<CombinedHotkey> m_CooldownHotkey = new();

        /// <summary>
        /// Check if the button hotkey will be called.
        /// NOTICE: this function should be called manually in project.
        /// </summary>
        public static void OnProcessHotkey()
        {
            // normal buttons
            foreach (var keyButton in m_RegisteredButton)
            {
                // cooldown
                bool cooldown = false;
                foreach (var cooldownKey in m_CooldownHotkey)
                {
                    if (keyButton.Key.Equals(cooldownKey))
                    {
                        cooldown = true;
                        break;
                    }
                }
                if (cooldown) continue;
                // invoke
                bool valid = true;
                for (int i = 0; i < keyButton.Key.Hotkeys.Count; i++)
                {
                    if (Input.IsKeyPressed(keyButton.Key.Hotkeys[i]) == false)
                    {
                        valid = false;
                        continue;
                    }
                }
                if (valid)
                {
                    if (keyButton.Value.Count > 0)
                    {
                        var count = keyButton.Value.Count;
                        if (keyButton.Value[count - 1].Pressed != null)
                        {
                            keyButton.Value[count - 1].Pressed();
                        }
                        m_CooldownHotkey.Add(keyButton.Key);
                    }
                }
            }

            // reset cooldown
            var resetCooldownList = new List<CombinedHotkey>();
            foreach (var key in m_CooldownHotkey)
            {
                for (int i = 0; i < key.Hotkeys.Count; i++)
                {
                    if (Input.IsKeyPressed(key.Hotkeys[i]) == false)
                    {
                        resetCooldownList.Add(key);
                        continue;
                    }
                }
            }
            for (int i = 0; i < resetCooldownList.Count; i++)
            {
                m_CooldownHotkey.Remove(resetCooldownList[i]);
            }
        }

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
                RegisterHotkey(combinedHotkey, this);
            }
            RegisterHotkey(new CombinedHotkey(HotkeyGroup1), this);
            RegisterHotkey(new CombinedHotkey(HotkeyGroup2), this);
        }

        private void UnregisterAllHotkey()
        {
            for (int i = 0; i < Hotkeys.Count; i++)
            {
                var combinedHotkey = new CombinedHotkey(Hotkeys[i]);
                UnregisterHotkey(combinedHotkey, this);
            }
            UnregisterHotkey(new CombinedHotkey(HotkeyGroup1), this);
            UnregisterHotkey(new CombinedHotkey(HotkeyGroup2), this);
        }

        private static void RegisterHotkey(CombinedHotkey combinedHotkey, BbxButton button)
        {
            if (combinedHotkey == null || combinedHotkey.Hotkeys.Count == 0)
                return;
            
            bool found = false;
            foreach (var keyButton in m_RegisteredButton)
            {
                if (keyButton.Key.Equals(combinedHotkey))
                {
                    found = true;
                    keyButton.Value.Add(button);
                }
            }
            if (found == false)
            {
                var list = new List<BbxButton>();
                list.Add(button);
                var key = combinedHotkey.Clone();
                m_RegisteredButton.Add(key, list);
            }
        }

        private static void UnregisterHotkey(CombinedHotkey combinedHotkey, BbxButton button)
        {
            if (combinedHotkey == null || combinedHotkey.Hotkeys.Count == 0)
                return;
            
            CombinedHotkey key = default;
            foreach (var keyButton in m_RegisteredButton)
            {
                if (keyButton.Key.Equals(combinedHotkey))
                {
                    key = keyButton.Key;
                    keyButton.Value.Remove(button);
                    break;
                }
            }
            if (key != null && m_RegisteredButton[key].Count == 0)
            {
                m_RegisteredButton.Remove(key);
            }
        }
        #endregion
    }
}
