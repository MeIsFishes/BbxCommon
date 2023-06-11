using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public partial class DcgGameEngine
    {
        private GameStage CreateDungeonStage()
        {
            var dungeonStage = StageWrapper.CreateStage("Dungeon Stage");
            dungeonStage.AddLoadItem(new InitModelData());
            dungeonStage.AddLoadItem(new InitPlayerAndCharacter());
            dungeonStage.AddLoadItem(new InitRoomData());
            return dungeonStage;
        }

        private class InitModelData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                DataApi.SetData(Resources.Load<ModelAttributesData>("DndCardGame/Config/ModelAttributesData"));
            }

            void IStageLoad.Unload(GameStage stage)
            {
                DataApi.ReleaseData<ModelAttributesData>();
            }
        }

        private class InitPlayerAndCharacter : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var player = EntityCreator.CreatePlayerEntity();
                var character = EntityCreator.CreateCharacterEntity();
                player.GetRawComponent<PlayerSingletonRawComponent>().AddCharacter(character);
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
                playerComp.DestroyCharacterEntities();
                playerComp.GetEntity().Destroy();
            }
        }

        private class InitRoomData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                DataApi.SetData(Resources.Load<RoomData>("DndCardGame/Config/RoomData"));
                EcsApi.AddSingletonRawComponent<DungeonRoomSingletonRawComponent>();
            }

            void IStageLoad.Unload(GameStage stage)
            {
                DataApi.ReleaseData<RoomData>();
                EcsApi.RemoveSingletonRawComponent<DungeonRoomSingletonRawComponent>();
            }
        }
    }
}
