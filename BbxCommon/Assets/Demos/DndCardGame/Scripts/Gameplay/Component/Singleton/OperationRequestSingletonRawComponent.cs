using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class OperationRequestSingletonRawComponent : EcsSingletonRawComponent
    {
        /// <summary>
        /// 操作处理是否被要求阻塞
        /// </summary>
        public bool Blocked;
        public Queue<OperationBase> Operations = new();
    }
}
