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
            
            dungeonStage.AddScene("DcgDungeon");

            dungeonStage.SetUiScene(UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiDungeonScene"));

            dungeonStage.AddLoadItem(new InitModelData());
            dungeonStage.AddLoadItem(new InitPlayerAndCharacter());
            dungeonStage.AddLoadItem(new InitRoomData());

            dungeonStage.AddLateLoadItem(new InitDungeon());
            
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

        /// <summary>
        /// 初始化地牢关卡
        /// </summary>
        private class InitDungeon : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                // 创建初始房间
                var roomData = DataApi.GetData<RoomData>();
                var roomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var room = Object.Instantiate(roomData.RoomPrefab);
                room.transform.position = new Vector3();
                roomComp.AddRoom(room);

                // 将玩家拉过来
                var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
                var character = playerComp.Characters[0];
                character.GetGameObject().transform.position = roomComp.CurRoom.transform.position + roomData.CharacterOffset;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>().DestroyAllRooms();
            }
        }
    }
}
