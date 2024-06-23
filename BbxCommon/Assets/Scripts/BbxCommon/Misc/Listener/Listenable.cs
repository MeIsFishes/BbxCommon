using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public enum EListenableVariableEvent
    {
        Dirty,
        // this event should be dispatched manually
        Invalid,
    }

    public interface IListenable
    {
        public IMessageDispatcher<int> MessageDispatcher { get; }
    }

    public abstract class ListenableBase : PooledObject, IListenable
    {
        protected MessageHandler<int> m_MessageHandler = new();
        public IMessageDispatcher<int> MessageDispatcher => m_MessageHandler;
    }

    public class ListenableVariableDirtyMessageData<T> : MessageData
    {
        public T CurValue;
    }

    public class ListenableVariable<T> : ListenableBase
    {
        private T m_Value;

        public T Value => m_Value;

        public ListenableVariable() {  }

        public ListenableVariable(T value) { m_Value = value; }

        public static ListenableVariable<T> Create(T value)
        {
            var variable = ObjectPool<ListenableVariable<T>>.Alloc();
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
        /// Announce items which are listening the <see cref="ListenableVariable{T}"/> that its value has been changed.
        /// </summary>
        public void SetDirty()
        {
            var messageData = ObjectPool<ListenableVariableDirtyMessageData<T>>.Alloc();
            messageData.CurValue = m_Value;
            m_MessageHandler.Dispatch((int)EListenableVariableEvent.Dirty, messageData);
            messageData.CollectToPool();
        }

        /// <summary>
        /// You should manually call this function to announce listeners that this <see cref="ListenableVariable{T}"/> is invalid,
        /// for the <see cref="ListenableVariable{T}"/> may not collect or destruct itself when its owner is collected.
        /// As this function calls, all listeners will be removed.
        /// </summary>
        public void MakeInvalid()
        {
            m_MessageHandler.Dispatch((int)EListenableVariableEvent.Invalid, null);
            m_MessageHandler.RemoveAllListeners();
        }
    }
}
