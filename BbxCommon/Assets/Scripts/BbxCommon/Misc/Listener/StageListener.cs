using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BbxCommon
{
    public abstract class StageListenerBase : PooledObject
    {
        private List<ListenableItemListener> m_Listeners = new();

        public void OnLoad()
        {
            InitListener();
            for (int i = 0; i < m_Listeners.Count; i++)
            {
                m_Listeners[i].AddListener();
            }
        }

        public void OnUnload()
        {
            for (int i = 0; i < m_Listeners.Count; i++)
            {
                m_Listeners[i].TryRemoveListener();
            }
        }

        protected abstract void InitListener();

        protected ListenableItemListener AddVariableListener(ListenableBase listenTarget, EListenableVariableEvent listeningEvent, UnityAction<MessageData> callback)
        {
            var info = new ListenableItemListener(listenTarget, (int)listeningEvent, callback);
            m_Listeners.Add(info);
            return info;
        }

        protected ListenableItemListener AddVariableDirtyListener<T>(ListenableVariable<T> listenTarget, UnityAction<T> callback)
        {
            var info = new ListenableItemListener(listenTarget, (int)EListenableVariableEvent.Dirty, (messageData) =>
            {
                if (messageData is ListenableVariableDirtyMessageData<T> variableDirtyMessage)
                    callback(variableDirtyMessage.CurValue);
            });
            m_Listeners.Add(info);
            return info;
        }

        protected ListenableItemListener AddVariableInvalidListener<T>(ListenableVariable<T> listenTarget, UnityAction callback)
        {
            var info = new ListenableItemListener(listenTarget, (int)EListenableVariableEvent.Invalid, (messageData) =>
            {
                callback();
            });
            m_Listeners.Add(info);
            return info;
        }

        protected ListenableItemListener AddListener(IListenable listenTarget, int listeningEvent, UnityAction<MessageData> callback)
        {
            var info = new ListenableItemListener(listenTarget, listeningEvent, callback);
            m_Listeners.Add(info);
            return info;
        }
    }
}
