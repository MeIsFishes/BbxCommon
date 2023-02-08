using UnityEditor;
using UnityEngine;

using System.Text;
using System.Linq;

using CardGame.Effects;
using CardGame.GameData.Cards;
using CardGame.GameData.Decks;
using CardGame.GameData.Interfaces;
using CardGame.Editor;

using CardGame.Textures;

namespace CardGame.TestTools.Editor {
    /// <summary>
    /// This will check every cards & effects to see that if there are any problems on the data.
    /// </summary>
    public class CardGameDataTesting : EditorWindow {
        private CardGameDataTesting myWindow;

        private static string approveds;
        private static string denieds;

        private Vector2 scrollPos1, scrollPos2;

        [MenuItem("Easy Card Game/Test Tools/Data Testing", false, 1)]
        public static void Init() {
            approveds = "";
            denieds = "";
            var currentWindow = (CardGameDataTesting)GetWindow(typeof(CardGameDataTesting), false, "Easy Card Game - Data Testing");
            currentWindow.Show();
        }

        void OnGUI() {
            GUI.skin = EasyCardEditor.GuiSkin;

            if (myWindow == null) {
                myWindow = (CardGameDataTesting)GetWindow(typeof(CardGameDataTesting));
            }

            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);

            GUILayout.Label("Data Testing will test all the data such as decks, cards, effects, images. " +
                "If there is a mistake on your data, you will see it here.");

            GUILayout.Space(10);

            if (GUILayout.Button ("Starting data testing")) {
                StartTesting();
            }

            GUILayout.BeginHorizontal();

            scrollPos1 = GUILayout.BeginScrollView(scrollPos1);
            GUILayout.BeginVertical(GUILayout.Width(300));

            GUILayout.Label("Approveds");
            GUI.color = Color.green;
            GUILayout.Label(approveds);
            GUI.color = Color.white;

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);
            GUILayout.BeginVertical(GUILayout.Width(300));

            GUILayout.Label("Denieds");
            GUI.color = Color.red;
            GUILayout.Label(denieds);
            GUI.color = Color.white;

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
        }

        private void StartTesting () {
            TestCards(out StringBuilder approved, out StringBuilder denided);

            approveds = approved.ToString();
            denieds = denided.ToString();
        }

        private void AddLog (string log, StringBuilder builder) {
            builder.Append(string.Format ("\r\n {0}",log));
        }

        private void TestCards (out StringBuilder approved, out StringBuilder denied) {
            var cardFiles = Resources.LoadAll<TextAsset>("Cards");
            var effects = Resources.LoadAll<SkillEffect>("SkillEffects").ToList ();
            var deckFiles = Resources.LoadAll<TextAsset>("Decks");

            var avatarDuplication = TextureCollectionReader.Readers["Avatars"].Read();
            var deckImagesDuplication = TextureCollectionReader.Readers["DeckTextures"].Read();

            approved = new StringBuilder();
            denied = new StringBuilder();

            if (avatarDuplication != null) {
                AddLog(string.Format("Resources/Avatars folder has a duplication with image name: {0}. You cannot have multiple images with same name.", avatarDuplication), denied);
            }

            if (deckImagesDuplication != null) {
                AddLog(string.Format("Resources/DeckImages folder has a duplication with image name: {0}. You cannot have multiple images with same name.", deckImagesDuplication), denied);
            }

            int length = cardFiles.Length;

            for (int i = 0; i < length; i++) {
                EditorUtility.DisplayProgressBar("Testing Cards", string.Format("Testing {0}", cardFiles[i]), (i + 1f) / length);

                // check for avatar.
                bool isError = false;
                // load card.
                BaseCard baseCard = JsonUtility.FromJson<BaseCard>(cardFiles[i].text);
                if (baseCard != null) {
                    if (!TextureCollectionReader.Readers["Avatars"].Textures.ContainsKey (baseCard.Avatar)) {
                        AddLog(string.Format("Card named {0} has avatar {1}, but its not found on avatars folder", baseCard.Name, baseCard.Avatar), denied);
                        isError = true;
                    }

                    // check drop rate;
                    if (baseCard.DropRate < 1 || baseCard.DropRate > 100) {
                        AddLog(string.Format("Card named {0} has drop rate {1}, out of range. Valid range is 1-100", baseCard.Name, baseCard.DropRate), denied);
                        isError = true;
                    }

                    if (baseCard.CardType == CardTypes.Attacker) {
                        IAttacker attacker;

                        if (baseCard.CardInteractionType == CardInteractionTypes.Organic) {
                            OrganicAttackerCard organicAttacker = JsonUtility.FromJson<OrganicAttackerCard>(cardFiles[i].text);
                            attacker = organicAttacker;
                            if (organicAttacker.AttackType == AttackTypes.Both) {
                                if (organicAttacker.RangedEffectRange < 0) {
                                    AddLog(string.Format("Card named {0} has negative effect range on its ranged attack => {1}", baseCard.Name, organicAttacker.RangedEffectRange), denied);
                                    isError = true;
                                }

                                if (effects.Find(x => x.name.Equals(organicAttacker.RangedEffectParticle)) == null) {
                                    AddLog(string.Format("Card named {0} has skill effect for his ranged attack: {1}, but its not found on effects folder in Resources.", baseCard.Name, organicAttacker.RangedEffectParticle), denied);
                                    isError = true;
                                }
                            }
                        }
                        else {
                            SkillAttackerCard skillAttacker = JsonUtility.FromJson<SkillAttackerCard>(cardFiles[i].text);
                            attacker = skillAttacker;
                        }

                        for (int a=0, alength = attacker.Attacks.Length; a<alength; a++) {
                            if (attacker.Attacks [a].Power < 0 || attacker.Attacks[a].RangedPower < 0) {
                                AddLog(string.Format("Card named {0} has {1} attack with minus value.", baseCard.Name, attacker.Attacks[a].EffectType.ToString ()), denied);
                                isError = true;
                            }
                        }
                    }

                    if (baseCard.CardInteractionType == CardInteractionTypes.Organic) {
                        OrganicCard organicCard = JsonUtility.FromJson<OrganicCard>(cardFiles[i].text);

                        for (int a = 0, alength = organicCard.Resistances.Length; a < alength; a++) {
                            if (organicCard.Resistances[a].Power < 0) {
                                AddLog(string.Format("Card named {0} has {1} resistance with minus value.", baseCard.Name, organicCard.Resistances[a].EffectType.ToString()), denied);
                                isError = true;
                            }
                        }
                    }

                    if (baseCard.CardType == CardTypes.Healer) {
                        IHealer healer;

                        if (baseCard.CardInteractionType == CardInteractionTypes.Organic) {
                            var organicHealer = JsonUtility.FromJson<OrganicHealerCard>(cardFiles[i].text);
                            healer = organicHealer;
                        } else {
                            var skillHealer = JsonUtility.FromJson<SkillHealerCard>(cardFiles[i].text);
                            healer = skillHealer;
                        }

                        if (healer.Heal < 0) {
                            AddLog(string.Format("Card named {0} has a negative heal effect range on its effect => {1}", baseCard.Name, baseCard.EffectRange), denied);
                            isError = true;
                        }
                    }

                    if (baseCard.EffectRange < 0) {
                        AddLog(string.Format("Card named {0} has negative effect range on its effect => {1}", baseCard.Name, baseCard.EffectRange), denied);
                        isError = true;
                    }

                    if (effects.Find(x => x.name.Equals(baseCard.EffectParticle)) == null) {
                        AddLog(string.Format("Card named {0} has skill effect {1}, but its not found on effects folder in Resources.", baseCard.Name, baseCard.EffectParticle), denied);
                        isError = true;
                    }
                } else {
                    AddLog(string.Format("Card file {0} has invalid card data. It was not parsed.", cardFiles[i].name), denied);
                    isError = true;
                }

                if (isError) {
                    AddLog(string.Format("Card file {0} is not approved.", cardFiles[i].name), denied);
                } else {
                    AddLog(string.Format("Card file {0} is approved.", cardFiles[i].name), approved);
                }
            }


            // test decks
            length = deckFiles.Length;

            for (int i = 0; i < length; i++) {
                EditorUtility.DisplayProgressBar("Testing Decks", string.Format("Testing {0}", deckFiles[i]), (i + 1f) / length);

                // check for avatar.
                bool isError = false;
                // load card.
                Deck deck = JsonUtility.FromJson<Deck>(deckFiles[i].text);
                deck.Id = deckFiles[i].name;

                if (deck != null) {
                    if (!TextureCollectionReader.Readers["DeckTextures"].Textures.ContainsKey(deck.Avatar)) {
                        AddLog(string.Format("Deck named {0} has avatar (small icon) {1}, but its not found on Deck Images folder", deck.Id, deck.Avatar), denied);
                        isError = true;
                    }

                    if (!TextureCollectionReader.Readers["DeckTextures"].Textures.ContainsKey(deck.BigAvatar)) {
                        AddLog(string.Format("Deck named {0} has avatar (big image) {1}, but its not found on Deck Images folder", deck.Id, deck.BigAvatar), denied);
                        isError = true;
                    }
                } else {
                    AddLog(string.Format("Deck file {0} has invalid deck data. It was not parsed.", deckFiles[i].name), denied);
                    isError = true;
                }

                if (isError) {
                    AddLog(string.Format("Deck file {0} is not approved.", deckFiles[i].name), denied);
                } else {
                    AddLog(string.Format("Deck file {0} is approved.", deckFiles[i].name), approved);
                }
            }

            for (int i = 0, effectLength = effects.Count; i < effectLength; i++) {
                bool isError = false;
                if (!string.IsNullOrEmpty(effects[i].HitParticle)) {
                    if (effects.Find(x => x.name.Equals(effects[i].HitParticle)) == null) {
                        AddLog(string.Format("Skill Effect {0} has Hit Particle {1} is not empty, but not found on the effects folder.", effects[i].name, effects[i].HitParticle), denied);
                        isError = true;
                    }
                } 
                
                if (!isError) {
                    AddLog(string.Format("Skill Effect {0} is approved.", effects[i].name), approved);
                } else {
                    AddLog(string.Format("Skill Effect {0} is not approved.", effects[i].name), denied);
                }
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
