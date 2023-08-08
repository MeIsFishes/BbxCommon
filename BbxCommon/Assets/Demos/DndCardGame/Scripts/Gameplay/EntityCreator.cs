using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public static class EntityCreator
    {
        public static Entity CreatePlayerEntity()
        {
            var entity = EcsApi.CreateEntity();

            entity.AddRawComponent<PlayerSingletonRawComponent>();

            return entity;
        }

        public static Entity CreateCharacterEntity()
        {
            var entity = EcsApi.CreateEntity();

            // ������ʼ����
            var playerDeckComp = entity.AddRawComponent<CharacterDeckRawComponent>();
            for (int i = 0; i < 6; i++)
            {
                playerDeckComp.AddDice(Dice.Create(EDiceType.D4));
            }
            for (int i = 0; i < 4; i++)
            {
                playerDeckComp.AddDice(Dice.Create(EDiceType.D6));
            }

            // ��ʼ������
            var attributesComp = entity.AddRawComponent<AttributesRawComponent>();
            attributesComp.Strength = 3;
            attributesComp.Dexterity = 3;
            attributesComp.Constitution = 3;
            attributesComp.Intelligence = 3;
            attributesComp.Wisdom = 3;

            // �������component
            entity.AddRawComponent<WalkToRawComponent>();

            // ������GameObject
            var prefab = DataApi.GetData<PrefabData>().PrefabDic["Player"];
            var gameObject = Object.Instantiate(prefab);
            entity.AttachToGameObject(gameObject);

            // ����aspect
            entity.CreateRawAspect<WalkToRawAspect>();

            return entity;
        }

        public static Entity CreateMainCameraEntity()
        {
            var entity = EcsApi.CreateEntity();

            var cameraData = DataApi.GetData<CameraData>();

            entity.AddRawComponent<MainCameraSingletonRawComponent>();

            var gameObject = Object.Instantiate(cameraData.CameraPrefab);
            entity.AttachToGameObject(gameObject);

            return entity;
        }

        public static Entity CreateRoomEntity(Vector3 position)
        {
            var entity = EcsApi.CreateEntity();

            var roomData = DataApi.GetData<RoomData>();

            var spawnRoomShowComp = entity.AddRawComponent<SpawnRoomShowRawComponent>();
            spawnRoomShowComp.OriginalPos = position;

            entity.AddRawComponent<RoomRawComponent>();

            var gameObject = Object.Instantiate(roomData.RoomPrefab);
            entity.AttachToGameObject(gameObject);

            gameObject.transform.position = position;

            return entity;
        }
    }
}
