using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public class CombatStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Combat Stage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiCombatScene"));

            stage.AddLoadItem(new CombatStageInitPlayerData());
            stage.AddLoadItem(new CombatStageInitCombatInfo());

            stage.AddLateLoadItem(new CombatStageBindUi());

            stage.AddUpdateSystem<CauseDamageSystem>();
            stage.AddUpdateSystem<TakeDamageSystem>();
            stage.AddUpdateSystem<CombatRoundSystem>();
            stage.AddUpdateSystem<CombatMonsterTurnSystem>();
            stage.AddUpdateSystem<CombatBeginTurnSystem>();
            stage.AddUpdateSystem<CombatEndTurnSystem>();
            stage.AddUpdateSystem<MonsterTurnSystem>();

            return stage;
        }

        private class CombatStageInitPlayerData : IStageLoad
        {
            // Ŀǰ������ʱ����component��������ʱ�ǿ��Եģ�����������һ��δ�����ܳ����ٻ���������
            // ������õ�����Ӧ���ǰ�ս���е�entity�͵�ͼ�е�entity��Ϊ����entityд��EntityCreator����
            void IStageLoad.Load(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                var charcterDeckComp = playerComp.Characters[0].GetRawComponent<CharacterDeckRawComponent>();
                var combatDeckComp = playerComp.Characters[0].AddRawComponent<CombatDeckRawComponent>();
                playerComp.Characters[0].AddRawComponent<CombatTurnRawComponent>();
                combatDeckComp.DicesInDeck.Clear();
                combatDeckComp.DicesInDeck.AddList(charcterDeckComp.Dices);
                combatDeckComp.DicesInDeck.Shuffle();
            }

            void IStageLoad.Unload(GameStage stage)
            {
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                var characterEntity = playerComp.Characters[0];
                characterEntity.RemoveRawComponent<CombatDeckRawComponent>();
                characterEntity.RemoveRawComponent<CombatTurnRawComponent>();
            }
        }

        private class CombatStageInitCombatInfo : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                // ��������
                var dungeonRoomComp = EcsApi.GetSingletonRawComponent<DungeonRoomSingletonRawComponent>();
                var roomData = DataApi.GetData<RoomData>();
                var roomPos = dungeonRoomComp.CurRoom.GetGameObject().transform.position;
                var spawnPos = roomPos + roomData.MonsterOffset;
                var monster = EntityCreator.CreateMonsterEntity(DataApi.GetData<MonsterData>(10010001), spawnPos, Quaternion.LookRotation((roomPos - spawnPos).SetY(0)));

                // ��ʼ��ս����Ϣ
                var combatInfoComp = EcsApi.AddSingletonRawComponent<CombatInfoSingletonRawComponent>();
                var combatRoundComp = EcsApi.AddSingletonRawComponent<CombatRoundSingletonRawComponent>();
                var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
                combatInfoComp.Character = playerComp.Characters[0];
                combatInfoComp.Monster = monster;
                combatRoundComp.EnterCombat = true;
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.RemoveSingletonRawComponent<CombatInfoSingletonRawComponent>();
                EcsApi.RemoveSingletonRawComponent<CombatRoundSingletonRawComponent>();
            }
        }

        private class CombatStageBindUi : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
                UiApi.GetUiController<UiCharacterInfoController>().Bind(combatInfoComp.Character);
                UiApi.GetUiController<UiMonsterInfoController>().Bind(combatInfoComp.Monster);
            }

            void IStageLoad.Unload(GameStage stage)
            {

            }
        }
    }
}
