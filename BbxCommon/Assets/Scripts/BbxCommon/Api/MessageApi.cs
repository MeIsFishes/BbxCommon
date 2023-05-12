using UnityEngine.Events;

namespace BbxCommon
{
    public static class MessageApi
    {
        /// <summary>
        /// A global message handler which simply process int key messages.
        /// </summary>
        public static MessageHandler<int> GlobalMessageDispatcher = new();
    }
}
