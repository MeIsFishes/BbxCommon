using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using Cin.Ui;

namespace Cin
{
    public class CinGameEngine : GameEngineBase<CinGameEngine>
    {
        protected override void OnAwake()
        {
            var uiScene = UiSceneWrapper.CreateUiScene<UiCinScene>();

            var stage = StageWrapper.CreateStage("Global Stage");
            stage.AddScene("CinMain");
            stage.SetUiScene(uiScene, Resources.Load<UiSceneAsset>("Cinemachine/Config/UiScene"));
            stage.AddUpdateSystem<PlayerMoveSystem>();

            StageWrapper.LoadStage(stage);
        }

        protected override void InitSingletonComponents()
        {
            EcsApi.AddSingletonRawComponent<CameraDataSingletonRawComponent>();
        }
    }
}
