using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using static UiCharacterStateContainerController;

namespace Dcg
{
    public class DungeonStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Dungeon Stage");
            
            stage.AddScene("DcgDungeon");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiDungeonScene"));

            stage.AddLoadItem<InitPlayerAndCharacter>();

            stage.AddLateLoadItem<InitDungeon>();

            stage.AddUpdateSystem<DungeonCameraSystem>();
            stage.AddUpdateSystem<SpawnRoomSystem>();
            stage.AddUpdateSystem<SpawnRoomShowSystem>();
            stage.AddUpdateSystem<CastSkillSystem>();
            
            return stage;
        }

        private class InitPlayerAndCharacter : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var player = EntityCreator.CreatePlayerEntity();
                var character = EntityCreator.CreateCharacterEntity();
                player.GetRawComponent<LocalPlayerSingletonRawComponent>().AddDungeonCharacter(character);
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                playerComp.DestroyCharacterEntities();
                playerComp.GetEntity().Destroy();
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
                var roomComp = EcsApi.AddSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var room = EntityCreator.CreateRoomEntity(Vector3.zero);
                roomComp.AddRoom(room);

                // 将玩家拉过来
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                var character = playerComp.DungeonEntities[0];
                character.GetGameObject().transform.position = roomComp.CurRoom.GetGameObject().transform.position + roomData.CharacterOffset;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>().DestroyAllRooms();
                EcsApi.RemoveSingletonRawComponent<DungeonRoomSingletonRawComponent>();
            }
        }
    }
}
