using System.Collections.Generic;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class ProcessOperationSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var operationComp = EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>();
            // 每帧只处理一条操作请求，以给出完整的一帧，令其他system tick的时候可以请求阻塞
            if (operationComp.BlockedOperations.Count > 0 && operationComp.Blocked == false)
            {
                var operation = operationComp.BlockedOperations.Dequeue();
                operation.Enter();
                operationComp.UpdatingOperations.Add(operation);
            }

            // 处理自由操作
            if (operationComp.FreeOperations.Count > 0)
            {
                var operation = operationComp.FreeOperations.Dequeue();
                operation.Enter();
                operationComp.UpdatingOperations.Add(operation);
            }

            // 处理当前挂起的operation的tick
            var finishedOperations = SimplePool<List<int>>.Alloc();
            for (int i = 0; i < operationComp.UpdatingOperations.Count; i++)
            {
                if (operationComp.UpdatingOperations[i].Update(UnityEngine.Time.deltaTime) == EOperationState.Finished)
                    finishedOperations.Add(i);
            }

            // 处理operation退出
            for (int i = finishedOperations.Count - 1; i >= 0; i--)
            {
                operationComp.UpdatingOperations[finishedOperations[i]].Exit();
                operationComp.UpdatingOperations.UnorderedRemoveAt(finishedOperations[i]);
            }

            finishedOperations.CollectToPool();
        }
    }
}
