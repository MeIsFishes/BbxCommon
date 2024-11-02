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

            stage.AddLateLoadItem(new BuildTestTask());
            stage.AddLoadItem(new InitSingletonComponent());
            stage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Config/DcgInteractingDataAsset"));

            stage.AddLateLoadItem(new InitCamera());
            stage.AddLateLoadItem(new BuildRandomPool());

            stage.AddUpdateSystem<ProcessOperationSystem>();

            return stage;
        }

        private class BuildTestTask : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                // generate
                var taskTest = TaskApi.CreateTaskInfo<TaskTestContext>("Test", 0);
                var taskDebugLogNode = taskTest.CreateTaskValueInfo<TaskDebugLogNode>(0);
                taskDebugLogNode.AddFieldInfo(TaskDebugLogNode.EField.Content, "Task log succeeded!");
                TaskApi.RegisterTask("Test", taskTest);

                // run task
                var context = new TaskTestContext();
                TaskApi.RunTask("Test", context);
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
