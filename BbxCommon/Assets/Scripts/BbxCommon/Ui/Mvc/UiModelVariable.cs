using UnityEngine;

namespace BbxCommon.Ui
{
    public enum EUiModelVariableEvent
    {
        Dirty,
        // todo: when model is destroyed or collected, dispatch destruction event
        Destroy,
    }

    public class UiModelVariableDirtyMessageData<T> : MessageData
    {
        public T CurValue;
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
            if (m_Value == null || m_Value.Equals(value) == false)
            {
                m_Value = value;
                SetDirty();
            }
        }

        /// <summary>
        /// Announce items which are listening the <see cref="UiModelVariable{T}"/> that its value has been changed.
        /// </summary>
        public void SetDirty()
        {
            var messageData = ObjectPool<UiModelVariableDirtyMessageData<T>>.Alloc();
            messageData.CurValue = m_Value;
            m_MessageHandler.Dispatch((int)EUiModelVariableEvent.Dirty, messageData);
            messageData.CollectToPool();
        }
    }
}
