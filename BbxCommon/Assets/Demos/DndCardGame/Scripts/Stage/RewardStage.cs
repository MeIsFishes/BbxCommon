using BbxCommon.Ui;
using BbxCommon;
using UnityEngine;

namespace Dcg
{
    public class RewardStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Reward Stage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiRewardScene"));

            stage.AddLoadItem(new RewardStageGenerateRewardDice());

            return stage;
        }

        private class RewardStageGenerateRewardDice : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var rewardDicesComp = EcsApi.AddSingletonRawComponent<RewardDicesSingletonRawComponent>();
                rewardDicesComp.Dices.ModifyCount(3);
                for (int i = 0; i < rewardDicesComp.Dices.Count; i++)
                {
                    rewardDicesComp.Dices[i] = Dice.Create(GameUtility.RandomPool.GetRandomRewardDiceType());
                }
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.RemoveSingletonRawComponent<RewardDicesSingletonRawComponent>();
            }
        }
    }
}
