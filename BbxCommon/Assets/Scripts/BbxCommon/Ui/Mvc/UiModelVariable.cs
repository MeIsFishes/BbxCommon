using UnityEngine;

namespace BbxCommon.Ui
{
    public enum EUiModelVariableEvent
    {
        Dirty,
        Destroy,
    }

    public class UiModelVariable<T> : UiModelItemBase
    {
        private T m_Value;

        public T Value => m_Value;

        public UiModelVariable() { }

        public static UiModelVariable<T> Create(T value)
        {
            var variable = ObjectPool<UiModelVariable<T>>.Alloc();
            variable.m_Value = value;
            return variable;
        }

        public void SetValue(T value)
        {
            m_Value = value;
            SetDirty();
        }

        public void SetDirty()
        {
            m_MessageHandler.Dispatch((int)EUiModelVariableEvent.Dirty);
        }
    }
}
