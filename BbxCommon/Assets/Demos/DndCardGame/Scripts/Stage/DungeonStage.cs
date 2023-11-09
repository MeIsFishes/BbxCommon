using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class DungeonStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Dungeon Stage");
            
            stage.AddScene("DcgDungeon");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiDungeonScene"));

            stage.AddLoadItem(new InitModelData());
            stage.AddLoadItem(new InitPlayerAndCharacter());
            stage.AddLoadItem(new InitRoomData());

            stage.AddLateLoadItem(new InitDungeon());

            stage.AddUpdateSystem<DungeonCameraSystem>();
            stage.AddUpdateSystem<SpawnRoomSystem>();
            stage.AddUpdateSystem<SpawnRoomShowSystem>();
            stage.AddUpdateSystem<CastSkillSystem>();
            
            return stage;
        }

        private class InitModelData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var modelAttributesData = Resources.Load<ModelAttributesData>("DndCardGame/Config/ModelAttributesData");
                DataApi.SetData(Object.Instantiate(modelAttributesData));
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
                var roomData = Resources.Load<RoomData>("DndCardGame/Config/RoomData");
                DataApi.SetData(Object.Instantiate(roomData));
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
                var room = EntityCreator.CreateRoomEntity(Vector3.zero);
                roomComp.AddRoom(room);

                // 将玩家拉过来
                var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
                var character = playerComp.Characters[0];
                character.GetGameObject().transform.position = roomComp.CurRoom.GetGameObject().transform.position + roomData.CharacterOffset;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>().DestroyAllRooms();
            }
        }
    }
}
