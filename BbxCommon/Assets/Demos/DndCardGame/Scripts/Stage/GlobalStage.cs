using System.Collections.Generic;
using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class GlobalStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Global Stage");

            stage.AddScene("DcgMain");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiGlobalScene"));

            stage.AddLoadItem<InitSingletonComponent>();
            stage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Config/DcgInteractingDataAsset"));
            
            stage.AddLateLoadItem<BuildTestTask>();
            stage.AddLateLoadItem<BuildTestBtTask>();
            stage.AddLateLoadItem<InitCamera>();
            stage.AddLateLoadItem<BuildRandomPool>();

            stage.AddUpdateSystem<ProcessOperationSystem>();

            return stage;
        }

        private class BuildTestTask : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var context = ObjectPool<TaskContextTest>.Alloc();
                TaskApi.RunTask("Test", context);
                context.CollectToPool();
            }

            void IStageLoad.Unload(GameStage stage)
            {

            }
        }

        private class BuildTestBtTask : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                // 1. 创建 TaskGroupInfo
                var groupInfo = TaskApi.CreateTaskInfo<TaskContextTest>("CodeTest", 0);

                // 2. 创建 TaskValueInfo 节点
                // 0: root
                var rootInfo = groupInfo.CreateTaskValueInfo<TaskBtRoot>(0);
                rootInfo.AddTaskConnectPoint(TaskBtRoot.EField.Tasks, new int[]{1});

                 // 1: sequence
                var sequence = groupInfo.CreateTaskValueInfo<TaskNodeSequence>(1);
                sequence.AddTaskConnectPoint(TaskBtRoot.EField.Tasks, new int[]{2, 3});

                // 2: log1
                var debugLogInfo1 = groupInfo.CreateTaskValueInfo<TaskNodeDebugLog>(2);
                debugLogInfo1.AddFieldInfo(TaskNodeDebugLog.EField.Content, "step1");

                // 3: log2
                var debugLogInfo2 = groupInfo.CreateTaskValueInfo<TaskNodeDebugLog>(3);
                debugLogInfo2.AddFieldInfo(TaskNodeDebugLog.EField.Content, "step2");

                TaskApi.RegisterTask("TestBt", groupInfo);

                var context = ObjectPool<Dcg.TaskContextTest>.Alloc();
                TaskApi.RunTask("TestBt", context);
                context.CollectToPool();
            }

            void IStageLoad.Unload(GameStage stage)
            {

            }
        }

        private class InitSingletonComponent : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                EcsApi.AddSingletonRawComponent<OperationRequestSingletonRawComponent>();
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.RemoveSingletonRawComponent<OperationRequestSingletonRawComponent>();
            }
        }

        private class InitCamera : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                EntityCreator.CreateMainCameraEntity();
            }

            void IStageLoad.Unload(GameStage stage)
            {

            }
        }

        private class BuildRandomPool : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                GameUtility.RandomPool.BuildMonsterDataPool();
                GameUtility.RandomPool.BuildDicePool();
            }

            void IStageLoad.Unload(GameStage stage)
            {

            }
        }
    }
}
