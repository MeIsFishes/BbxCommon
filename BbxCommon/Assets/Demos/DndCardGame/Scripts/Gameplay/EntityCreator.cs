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

            // 创建初始卡组
            var playerDeckComp = entity.AddRawComponent<CharacterDeckRawComponent>();
            for (int i = 0; i < 6; i++)
            {
                playerDeckComp.Dices.Add(Dice.Create(EDiceType.D4));
            }
            for (int i = 0; i < 4; i++)
            {
                playerDeckComp.Dices.Add(Dice.Create(EDiceType.D6));
            }

            // 初始化属性
            var attributesComp = entity.AddRawComponent<AttributesRawComponent>();
            attributesComp.Strength = 3;
            attributesComp.Dexterity = 3;
            attributesComp.Constitution = 3;
            attributesComp.Intelligence = 3;
            attributesComp.Wisdom = 3;

            return entity;
        }
    }
}
