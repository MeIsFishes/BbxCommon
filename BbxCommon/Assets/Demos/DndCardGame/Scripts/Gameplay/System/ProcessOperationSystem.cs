using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public class ProcessOperationSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var operationComp = EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>();
            if (operationComp.Blocked)
                return;

            // 每帧只处理一条操作请求，以给出完整的一帧，令其他system tick的时候可以请求阻塞
            operationComp.Operations.Dequeue().OnProcess();
        }
    }
}
