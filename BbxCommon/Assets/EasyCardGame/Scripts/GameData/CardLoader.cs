using UnityEngine;
using CardGame.GameData.Cards;
using CardGame.GameData.Interfaces;

namespace CardGame.GameData {
    public class CardLoader {
        public CardLoader (string data,
                    out BaseCard loadedCard,
                    out IVulnerable vulnerableCard,
                    out IAttacker attackerCard,
                    out IHealer healerCard,
                    out IOrganicAttacker organicAttackerCard) {

            var card = JsonUtility.FromJson<BaseCard>(data);

            attackerCard = null;
            healerCard = null;
            vulnerableCard = null;
            loadedCard = null;
            organicAttackerCard = null;

            if (card.CardInteractionType == CardInteractionTypes.Organic) {
                if (card.CardType == CardTypes.Attacker) {
                    var attacker = JsonUtility.FromJson<OrganicAttackerCard>(data);
                    vulnerableCard = attacker;
                    attackerCard = attacker;
                    organicAttackerCard = attacker;
                    loadedCard = attacker;
                }

                if (card.CardType == CardTypes.Healer) {
                    var healer = JsonUtility.FromJson<OrganicHealerCard>(data);
                    vulnerableCard = healer;
                    healerCard = healer;
                    loadedCard = healer;
                }
            } else if (card.CardInteractionType == CardInteractionTypes.Skill) {
                if (card.CardType == CardTypes.Attacker) {
                    var attacker = JsonUtility.FromJson<SkillAttackerCard>(data);
                    attackerCard = attacker;
                    loadedCard = attacker;
                }

                if (card.CardType == CardTypes.Healer) {
                    var healer = JsonUtility.FromJson<SkillHealerCard>(data);
                    healerCard = healer;
                    loadedCard = healer;
                }
            }
        }
    }
}

