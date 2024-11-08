using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public static class EntityCreator
    {
        public static Entity CreatePlayerEntity()
        {
            var entity = EcsApi.CreateEntity();

            entity.AddRawComponent<LocalPlayerSingletonRawComponent>();

            return entity;
        }

        public static Entity CreateCharacterEntity()
        {
            var entity = EcsApi.CreateEntity();


            // 创建初始卡组
            var playerDeckComp = entity.AddRawComponent<CharacterDeckRawComponent>();
            for (int i = 0; i < 6; i++)
            {
                playerDeckComp.AddDice(Dice.Create(EDiceType.D4));
            }
            for (int i = 0; i < 4; i++)
            {
                playerDeckComp.AddDice(Dice.Create(EDiceType.D6));
            }

            // 初始化属性
            var attributesComp = entity.AddRawComponent<AttributesRawComponent>();
            attributesComp.MaxHp = 28;
            attributesComp.CurHp = 28;
            attributesComp.ArmorClass.Add(EDiceType.D8);
            attributesComp.ArmorClassVariable.SetDirty();
            attributesComp.Strength = 3;
            attributesComp.Dexterity = 3;
            attributesComp.Constitution = 3;
            attributesComp.Intelligence = 3;
            attributesComp.Wisdom = 3;
            attributesComp.UnitTags.Add("NoArmor");

            // 添加其他component
            entity.AddRawComponent<WalkToRawComponent>();
            entity.AddRawComponent<AttackableRawComponent>();
            entity.AddRawComponent<CastSkillRawComponent>();

            // 关联到GameObject
            var prefab = DataApi.GetData<PrefabData>().PrefabDic["Player"];
            var gameObject = Object.Instantiate(prefab);
            entity.BindGameObject(gameObject);

            // 创建aspect
            entity.CreateRawAspect<WalkToRawAspect>();

            return entity;
        }
        public static Entity CreateCombatEntity()
        {
            var entity = EcsApi.CreateEntity();
            // 创建初始卡组
            var combatDeckComp = entity.AddRawComponent<CombatDeckRawComponent>();
            combatDeckComp.DicesInDeck.Clear();
            // 初始化属性
            var attributesComp = entity.AddRawComponent<AttributesRawComponent>();
            attributesComp.MaxHp = 28;
            attributesComp.CurHp = 28;
            attributesComp.ArmorClass.Add(EDiceType.D8);
            attributesComp.ArmorClassVariable.SetDirty();
            attributesComp.Strength = 3;
            attributesComp.Dexterity = 3;
            attributesComp.Constitution = 3;
            attributesComp.Intelligence = 3;
            attributesComp.Wisdom = 3;

            // 添加其他component
            entity.AddRawComponent<WalkToRawComponent>();
            entity.AddRawComponent<AttackableRawComponent>();
            entity.AddRawComponent<CastSkillRawComponent>();

            // 关联到GameObject
            var prefab = DataApi.GetData<PrefabData>().PrefabDic["Player"];
            var gameObject = Object.Instantiate(prefab);
            entity.BindGameObject(gameObject);
            //绑定回合
            entity.AddRawComponent<CombatTurnRawComponent>();
            // 创建aspect
            entity.CreateRawAspect<WalkToRawAspect>();
            //UI
            entity.BindHud<HudCharacterStatusController>();
            entity.BindHud<HudCommonStatusController>(); 

            return entity;
        }

        public static Entity CreateMonsterEntity(MonsterData monsterData, Vector3 position, Quaternion rotation)
        {
            var entity = EcsApi.CreateEntity();

            // 初始化属性
            var attributesComp = entity.AddRawComponent<AttributesRawComponent>();
            attributesComp.MaxHp = monsterData.HitPoints;
            attributesComp.CurHp = monsterData.HitPoints;
            attributesComp.ArmorClass.AddList(monsterData.ArmorClass);
            attributesComp.ArmorClassVariable.SetDirty();
            attributesComp.Strength = monsterData.Strength;
            attributesComp.Dexterity = monsterData.Dexterity;
            attributesComp.Constitution = monsterData.Constitution;
            attributesComp.Intelligence = monsterData.Intelligence;
            attributesComp.Wisdom = monsterData.Wisdom;
            attributesComp.UnitTags.AddList(monsterData.ArmorTags);

            // 初始化怪物属性
            var monsterComp = entity.AddRawComponent<MonsterRawComponent>();
            monsterComp.Name = monsterData.Name;
            monsterComp.AttackDices.AddList(monsterData.AttackDices);
            monsterComp.DamageDices.AddList(monsterData.DamageDices);
            monsterComp.Modifier = monsterData.AttackModifier;

            // 添加其他component
            entity.AddRawComponent<AttackableRawComponent>();
            entity.AddRawComponent<CombatTurnRawComponent>();
            entity.AddRawComponent<AiBehaviourRawComponent>();
            entity.AddRawComponent<CastSkillRawComponent>();

            // 关联到GameObject
            var gameObject = Object.Instantiate(monsterData.Prefab);
            entity.BindGameObject(gameObject);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            // 绑定HUD
            entity.BindHud<HudMonsterStatusController>();
            entity.BindHud<HudCommonStatusController>();

            return entity;
        }

        public static Entity CreateMainCameraEntity()
        {
            var entity = EcsApi.CreateEntity();

            var cameraData = DataApi.GetData<CameraData>();

            entity.AddRawComponent<MainCameraSingletonRawComponent>();

            var gameObject = Object.Instantiate(cameraData.CameraPrefab);
            entity.BindGameObject(gameObject);

            return entity;
        }

        public static Entity CreateRoomEntity(Vector3 position)
        {
            var entity = EcsApi.CreateEntity();

            var roomData = DataApi.GetData<RoomData>();

            var spawnRoomShowComp = entity.AddRawComponent<SpawnRoomShowRawComponent>();
            GameUtility.Room.SpawnRoomStart(entity, position);

            entity.AddRawComponent<RoomRawComponent>();

            var gameObject = Object.Instantiate(roomData.RoomPrefab);
            entity.BindGameObject(gameObject);

            gameObject.transform.position = position;

            return entity;
        }
    }
}
