using UnityEngine;

namespace BbxCommon.Ui
{
    public interface IModelItemHost
    {
        void SetDirty();
    }

    public class UiModelItem<T> : PooledObject, IModelItemHost
    {
        private T m_Value;
        private IModelItemHost m_Host;

        /// <summary>
        /// There should be only <see cref="SimpleMessageListener"/> adding to the <see cref="MessageHandler"/>,
        /// so that we don't care about the message key.
        /// </summary>
        internal MessageHandler<int> MessageHandler = new();

        public T Value => m_Value;

        public UiModelItem(IModelItemHost host, T value = default(T))
        {
            m_Host = host;
            m_Value = value;
        }

        public void SetValue(T value)
        {
            m_Value = value;
            SetDirty();
        }

        public void SetDirty()
        {
            MessageHandler.Dispatch(0);
            m_Host.SetDirty();
        }
    }
}
