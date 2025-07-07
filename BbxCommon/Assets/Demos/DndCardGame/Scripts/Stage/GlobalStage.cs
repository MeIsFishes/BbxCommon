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
            stage.AddLateLoadItem<InitCamera>();
            stage.AddLateLoadItem<BuildRandomPool>();

            stage.AddUpdateSystem<ProcessOperationSystem>();

            return stage;
        }

        private class BuildTestTask : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                // generate
                var taskTest = TaskApi.CreateTaskInfo<TaskContextTest>("CodeTest", 0);
                var timelineInfo = taskTest.CreateTaskTimelineValueInfo(0, 5f);
                timelineInfo.AddTimelineInfo(0f, 0f, 1);
                timelineInfo.AddTimelineInfo(5f, 0f, 2);

                var debugLogInfo1 = taskTest.CreateTaskValueInfo<TaskNodeDebugLog>(1);
                debugLogInfo1.AddFieldInfoFromContext(TaskNodeDebugLog.EField.Content, TaskContextTest.EField.DebugContent);
                debugLogInfo1.AddEnterCondition(3); // EnterCondition blocks entering, there will be no output.

                var debugLogInfo2 = taskTest.CreateTaskValueInfo<TaskNodeDebugLog>(2);
                debugLogInfo2.AddFieldInfo(TaskNodeDebugLog.EField.Content, "5s later!");
                debugLogInfo2.AddFieldInfoFromBlackboard(TaskNodeDebugLog.EField.BlackLong, TaskContextTest.EField.BlackLong);
                debugLogInfo2.AddFieldInfoFromBlackboard(TaskNodeDebugLog.EField.BlackDouble, TaskContextTest.EField.BlackDouble);
                debugLogInfo2.AddFieldInfoFromBlackboard(TaskNodeDebugLog.EField.BlackObject, TaskContextTest.EField.BlackObject);
                debugLogInfo2.AddCondition(4);  // Normal Condition doesn't block entering. For the node output via OnEnter(), there will be output in console.

                var greaterInfo1 = taskTest.CreateTaskValueInfo<TaskConditionGreaterThan5>(3);
                greaterInfo1.AddFieldInfoFromContext(TaskConditionGreaterThan5.EField.Num, TaskContextTest.EField.Num);

                var greaterInfo2 = taskTest.CreateTaskValueInfo<TaskConditionGreaterThan5>(4);
                greaterInfo2.AddFieldInfo(TaskConditionGreaterThan5.EField.Num, 1);

                // run task
                var context = ObjectPool<TaskContextTest>.Alloc();
                context.DebugContent = "Task log succeeded!";
                context.Num = 4;
                context.SetBlackBoardLongValue("BlackLong", 123);
                context.SetBlackBoardDoubleValue("BlackDouble", 456.123);
                context.SetBlackBoardObjectValue("BlackObject", new List<int>()
                {
                    6,7,8
                });
                TaskApi.RunTask("Test", context);
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
