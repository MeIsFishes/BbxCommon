using UnityEngine;

namespace BbxCommon.Ui
{
    public enum EUiModelVariableEvent
    {
        Dirty,
        Destroy,
    }

    public class UiModeVariable<T> : UiModelItemBase
    {
        private T m_Value;
        private UiModelItemBase m_Host;

        /// <summary>
        /// There should be only <see cref="SimpleMessageListener"/> adding to the <see cref="m_MessageHandler"/>.
        /// </summary>
        private MessageHandler<int> m_MessageHandler = new();

        internal override IMessageDispatcher<int> MessageDispatcher => m_MessageHandler;

        public T Value => m_Value;

        public UiModeVariable(UiModelItemBase host, T value = default(T))
        {
            m_Host = host;
            m_Value = value;
        }

        public void SetValue(T value)
        {
            m_Value = value;
            SetDirty();
        }

        public override void SetDirty()
        {
            m_MessageHandler.Dispatch((int)EUiModelVariableEvent.Dirty);
            m_Host?.SetDirty();
        }
    }
}
