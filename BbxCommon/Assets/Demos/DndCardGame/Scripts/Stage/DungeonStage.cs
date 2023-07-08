using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class DungeonStage
    {
        public static GameStage CreateStage()
        {
            var dungeonStage = DcgGameEngine.Instance.StageWrapper.CreateStage("Dungeon Stage");
            
            dungeonStage.AddScene("DcgDungeon");

            dungeonStage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiDungeonScene"));

            dungeonStage.AddLoadItem(new InitModelData());
            dungeonStage.AddLoadItem(new InitPlayerAndCharacter());
            dungeonStage.AddLoadItem(new InitRoomData());

            dungeonStage.AddLateLoadItem(new InitDungeon());

            dungeonStage.AddUpdateSystem<DungeonCameraSystem>();
            
            return dungeonStage;
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
        /// ��ʼ�����ιؿ�
        /// </summary>
        private class InitDungeon : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                // ������ʼ����
                var roomData = DataApi.GetData<RoomData>();
                var roomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var room = Object.Instantiate(roomData.RoomPrefab);
                room.transform.position = new Vector3();
                roomComp.AddRoom(room);

                // �����������
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
