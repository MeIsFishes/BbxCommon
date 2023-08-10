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
            attributesComp.MaxHp = 16;
            attributesComp.CurHp = 16;
            attributesComp.ArmorClass.Add(EDiceType.D8);
            attributesComp.Strength = 3;
            attributesComp.Dexterity = 3;
            attributesComp.Constitution = 3;
            attributesComp.Intelligence = 3;
            attributesComp.Wisdom = 3;

            // �������component
            entity.AddRawComponent<WalkToRawComponent>();
            entity.AddRawComponent<AttackableRawComponent>();

            // ������GameObject
            var prefab = DataApi.GetData<PrefabData>().PrefabDic["Player"];
            var gameObject = Object.Instantiate(prefab);
            entity.AttachToGameObject(gameObject);

            // ����aspect
            entity.CreateRawAspect<WalkToRawAspect>();

            return entity;
        }

        public static Entity CreateMonsterEntity(MonsterData monsterData, Vector3 position, Quaternion rotation)
        {
            var entity = EcsApi.CreateEntity();

            // ��ʼ������
            var attributesComp = entity.AddRawComponent<AttributesRawComponent>();
            attributesComp.MaxHp = monsterData.HitPoints;
            attributesComp.CurHp = monsterData.HitPoints;
            attributesComp.ArmorClass.AddList(monsterData.ArmorClass);
            attributesComp.Strength = monsterData.Strength;
            attributesComp.Dexterity = monsterData.Dexterity;
            attributesComp.Constitution = monsterData.Constitution;
            attributesComp.Intelligence = monsterData.Intelligence;
            attributesComp.Wisdom = monsterData.Wisdom;

            // ��ʼ����������
            var monsterComp = entity.AddRawComponent<MonsterRawComponent>();
            monsterComp.AttackDices.AddList(monsterData.AttackDices);
            monsterComp.DamageDices.AddList(monsterData.DamageDices);

            // �������component
            entity.AddRawComponent<AttackableRawComponent>();

            // ������GameObject
            var gameObject = Object.Instantiate(monsterData.Prefab);
            entity.AttachToGameObject(gameObject);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

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
