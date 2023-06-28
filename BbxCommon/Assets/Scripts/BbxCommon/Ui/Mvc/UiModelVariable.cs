using UnityEngine;

namespace BbxCommon.Ui
{
    public enum EUiModelVariableEvent
    {
        Dirty,
        Destroy,
    }

    public class UiModelVariableDirtyMessageData<T> : MessageDataBase
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
            if (m_Value.Equals(value) == false)
            {
                m_Value = value;
                SetDirty();
            }
        }

        public void SetDirty()
        {
            var messageData = ObjectPool<UiModelVariableDirtyMessageData<T>>.Alloc();
            messageData.CurValue = m_Value;
            m_MessageHandler.Dispatch((int)EUiModelVariableEvent.Dirty, messageData);
            messageData.CollectToPool();
        }
    }
}
