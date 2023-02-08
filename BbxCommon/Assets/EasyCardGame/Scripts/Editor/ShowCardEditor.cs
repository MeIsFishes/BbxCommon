using UnityEngine;
using UnityEditor;

using System.IO;

using CardGame.GameData;
using CardGame.GameData.Cards;
using CardGame.GameData.Interfaces;

using System.Collections.Generic;
using System.Linq;

using CardGame.Textures;

namespace CardGame.Editor {
    public class ShowCardEditor : EditorWindow {
        private static string cardName;
        private static string[] skillEffects;
        private static Vector2 scrollPos;

        private static int currentAvatar;
        private static int currentEffect;
        private static int currentRangedEffect;
        private static int currentCounterEffect;
        private static AttackTypes currentAttackType;

        private static BaseCard loadedCard;
        private static IVulnerable loadedVulnerableCard;
        private static IAttacker loadedAttackerCard;
        private static IHealer loadedHealerCard;
        private static IOrganicAttacker loadedOrganicAttackerCard;

        private static bool showCardIdentiy;
        private static bool showCardColor;
        private static bool showSkill;
        private static bool showHealth;
        private static bool showResistances;
        private static bool showAttacks;
        private static bool showHealer;
        private static bool showCounter;

        public static BaseTexture CurrentTexture;

        private ShowCardEditor myWindow;

        public static void Init(string _loadedText, string _cardName, string[] _skillEffects) {
            var newLoader = new CardLoader(_loadedText,
                out loadedCard,
                out loadedVulnerableCard,
                out loadedAttackerCard,
                out loadedHealerCard,
                out loadedOrganicAttackerCard);

            cardName = _cardName;
            skillEffects = _skillEffects;

            TextureCollectionReader.Readers["Avatars"].Read();

            if (TextureCollectionReader.Readers["Avatars"].Textures.ContainsKey (loadedCard.Avatar)) {
                CurrentTexture = TextureCollectionReader.Readers["Avatars"].Textures[loadedCard.Avatar];
            }

            currentEffect = skillEffects.ToList().FindIndex(x => x.Equals(loadedCard.EffectParticle));

            if (loadedOrganicAttackerCard != null) {
                currentRangedEffect = skillEffects.ToList().FindIndex(x => x.Equals(loadedOrganicAttackerCard.RangedEffectParticle));
                currentAttackType = loadedOrganicAttackerCard.AttackType;
            }

            if (loadedVulnerableCard != null) {
                currentCounterEffect = skillEffects.ToList().FindIndex(x => x.Equals(loadedVulnerableCard.CounterEffectParticle));
            }

            if (currentAvatar == -1) {
                currentAvatar = 0;
            }

            if (currentRangedEffect == -1) {
                currentRangedEffect = 0;
            }

            if (currentEffect == -1) {
                currentEffect = 0;
            }

            var window = (ShowCardEditor)GetWindow(typeof(ShowCardEditor));
            window.titleContent = new GUIContent(string.Format("Showing card {0}", _cardName));
            window.minSize = new Vector2(400, 300);
            window.Show();

            showCardIdentiy = true;
        }

        private void OnDestroy() {
            CardEditorPreview.Clear();
        }

        private void OnGUI() {
            if (myWindow == null) {
                myWindow = (ShowCardEditor)GetWindow(typeof(ShowCardEditor));
            }

            GUI.skin = EasyCardEditor.GuiSkin;
            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);

            if (loadedCard == null)
                return;

            if (GUILayout.Button("Save", GUILayout.Height(30))) {
                createCard(false);
                AssetDatabase.Refresh();

                EasyCardEditor.LoadCards();
            }

            if (GUILayout.Button("Preview card on scene view")) {
                GUILayout.Label("Card preview enabled on scene view.");
                createCard(true);
            }

            GUI.color = Color.white;

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.Label(string.Format("{0}.json, Type: {1}, {2}", cardName, loadedCard.CardInteractionType, loadedCard.CardType));

            // drop rate
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("Drop Rate {0}", loadedCard.DropRate), EditorStyles.helpBox, GUILayout.Width(100));
            loadedCard.DropRate = EditorGUILayout.IntSlider(loadedCard.DropRate, 1, 100, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            GUILayout.Label(string.Format("If decks have empty slots, rest of the cards will be generated randomly. This rate will increase the selection rate of the card.", loadedCard.DropRate), EditorStyles.helpBox, GUILayout.Width(200));
            //

            GUILayout.Space(20);

            showCardIdentiy = EditorGUILayout.Foldout(showCardIdentiy, "Cards Identity");

            if (showCardIdentiy) {
                GUILayout.Space(10);
                GUILayout.Label("Unity rich text supported.");
                GUILayout.Label(" You can use <i>ITALIC</i> <b>BOLD</b> <color=orange>colored text</color> etc.");

                if (GUILayout.Button("For rich texts commands, check here.")) {
                    Application.OpenURL("https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html");
                }

                // name and desc.
                loadedCard.Name = EditorGUIHelper.DrawTextField("Name", ref loadedCard.Name);
                GUILayout.Label("Description", EditorStyles.helpBox);
                loadedCard.Desc = EditorGUILayout.TextArea(loadedCard.Desc, GUILayout.Height(50));
            }

            GUI.skin = null;
            showCardColor = EditorGUILayout.Foldout(showCardColor, "Cards Color");
            if (showCardColor) {
                loadedCard.RareColor = EditorGUILayout.ColorField(new GUIContent() { text = "RareColor" }, loadedCard.RareColor, true, false, true);
            }

            GUI.skin = EasyCardEditor.GuiSkin;
            // 

            const int maxRange = 11;

            showSkill = EditorGUILayout.Foldout(showSkill, "Cards Skill");

            if (showSkill) {
                // skill effect 
                GUILayout.BeginHorizontal();
                GUILayout.Label("Skill Effect", EditorStyles.helpBox, GUILayout.Width(70));
                currentEffect = EditorGUILayout.Popup(currentEffect, skillEffects);
                if (currentEffect >= 0 && currentEffect < skillEffects.Length)
                    loadedCard.EffectParticle = skillEffects[currentEffect];
                //
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();

                GUILayout.Label("Skill Effect Range", EditorStyles.helpBox, GUILayout.Width(100));
                loadedCard.EffectRange = EditorGUILayout.IntSlider(loadedCard.EffectRange, 0, maxRange / 2);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Effect both rows", EditorStyles.helpBox, GUILayout.Width(100));
                loadedCard.EffectBothRows = EditorGUILayout.Toggle(loadedCard.EffectBothRows, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                visualize(loadedCard.EffectRange);

                GUILayout.Space(20);

                // ranged skill effect is necessary. (Available only for Organic cards have "Both" attack types.
                if (loadedOrganicAttackerCard != null) {
                    if (loadedOrganicAttackerCard.AttackType == AttackTypes.Both) {
                        GUILayout.BeginHorizontal();
                        // ranged skill effect 
                        GUILayout.Label("Ranged Skill Effect", EditorStyles.helpBox, GUILayout.Width(70));
                        currentRangedEffect = EditorGUILayout.Popup(currentRangedEffect, skillEffects);
                        if (currentRangedEffect >= 0 && currentRangedEffect < skillEffects.Length)
                            loadedOrganicAttackerCard.RangedEffectParticle = skillEffects[currentRangedEffect];
                        //
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Ranged Effect both rows", EditorStyles.helpBox, GUILayout.Width(100));
                        loadedOrganicAttackerCard.RangedEffectBothRows = EditorGUILayout.Toggle(loadedOrganicAttackerCard.RangedEffectBothRows, GUILayout.Width(200));
                        GUILayout.EndHorizontal();


                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Ranged Skill Effect Range", EditorStyles.helpBox, GUILayout.Width(140));
                        loadedOrganicAttackerCard.RangedEffectRange = EditorGUILayout.IntSlider(loadedOrganicAttackerCard.RangedEffectRange, 0, maxRange / 2);

                        GUILayout.EndHorizontal();

                        visualize(loadedOrganicAttackerCard.RangedEffectRange);
                    }
                }
                //

                GUILayout.Space(10);

                void visualize(int val) {
                    // effect range visualization
                    GUILayout.BeginHorizontal();
                    for (int i = 0; i < 11; i++) {
                        if (i >= maxRange / 2 - val && i <= maxRange / 2 + val) {
                            GUI.color = Color.yellow;
                            GUILayout.Box("X");
                        } else {
                            GUI.color = new Color(1, 1, 1, 0.5f);
                            GUILayout.Box("0");
                        }
                    }
                    GUILayout.EndHorizontal();
                    //restore color.
                    GUI.color = Color.white;

                    GUILayout.Space(10);
                }
            }

            // make an array with effect types.
            int effectsLength = (int)Effect.EffectTypes.NumOfEffects;
            Effect.EffectTypes[] effects = new Effect.EffectTypes[effectsLength];
            for (int i = 0; i < effectsLength; i++) {
                effects[i] = ((Effect.EffectTypes)i);
            }

            Effect[] fixEffectsArray(Effect[] currentArr) {
                var list = new List<Effect>();
                if (currentArr == null) {
                    list = new List<Effect>();
                } else {
                    list = currentArr.ToList();
                }

                for (int i = 0; i < effectsLength; i++) {
                    if (list.Find(x => x.EffectType == effects[i]) == null) {
                        list.Add(new Effect(effects[i]));
                    }
                }

                return list.ToArray();
            }
            //

            #region drawing organic card
            if (loadedVulnerableCard != null) {
                showHealth = EditorGUILayout.Foldout(showHealth, "Cards Health");
                if (showHealth) {
                    int health;
                    var healthAsString = loadedVulnerableCard.Health.ToString();

                    if (int.TryParse(EditorGUIHelper.DrawTextField("Health", ref healthAsString), out health)) {
                        loadedVulnerableCard.Health = Mathf.Clamp(health, 1, int.MaxValue);
                    }

                    int points;
                    var pointsAsString = loadedVulnerableCard.Points.ToString();
                    if (int.TryParse(EditorGUIHelper.DrawTextField("Points", ref pointsAsString), out points)) {
                        loadedVulnerableCard.Points = Mathf.Clamp(points, 1, int.MaxValue);
                    }
                }

                showResistances = EditorGUILayout.Foldout(showResistances, "Resistances");
                if (showResistances) {
                    #region resistances
                    GUILayout.Label("Resistances");

                    var resistances = fixEffectsArray(loadedVulnerableCard.Resistances);

                    EditorGUILayout.BeginVertical();

                    for (int i = 0, length = resistances.Length; i < length; i++) {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label(resistances[i].EffectType.ToString(), EditorStyles.helpBox, GUILayout.Width(100));

                        int power;
                        var powerAsString = resistances[i].Power.ToString();

                        powerAsString = GUILayout.TextField(powerAsString, GUILayout.Width(30));

                        if (int.TryParse(powerAsString, out power))
                            resistances[i].Power = Mathf.Clamp(power, 0, int.MaxValue);

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();

                    loadedVulnerableCard.Resistances = resistances;
                    #endregion
                }

                showCounter = EditorGUILayout.Foldout(showCounter, "Counter Attack");
                if (showCounter) {
                    #region counter
                    GUILayout.Label("When a placed organic card get attack, it will attack back as a counter attack.", EditorStyles.helpBox, GUILayout.Width(340));

                    loadedVulnerableCard.CounterEnabled = EditorGUILayout.Toggle("Is Counter Attack Enabled?", loadedVulnerableCard.CounterEnabled);

                    if (loadedVulnerableCard.CounterEnabled) {

                        GUILayout.BeginHorizontal();
                        // counter effect.
                        GUILayout.Label("Counter Attack Skill Effect", EditorStyles.helpBox, GUILayout.Width(70));
                        currentCounterEffect = EditorGUILayout.Popup(currentCounterEffect, skillEffects);
                        if (currentCounterEffect >= 0 && currentCounterEffect < skillEffects.Length)
                            loadedVulnerableCard.CounterEffectParticle = skillEffects[currentCounterEffect];
                        //
                        GUILayout.EndHorizontal();

                        var counters = fixEffectsArray(loadedVulnerableCard.CounterEffects);

                        EditorGUILayout.BeginVertical();

                        for (int i = 0, length = counters.Length; i < length; i++) {
                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label(counters[i].EffectType.ToString(), EditorStyles.helpBox, GUILayout.Width(100));

                            int power;
                            var powerAsString = counters[i].Power.ToString();

                            powerAsString = GUILayout.TextField(powerAsString, GUILayout.Width(30));

                            if (int.TryParse(powerAsString, out power))
                                counters[i].Power = Mathf.Clamp(power, 0, int.MaxValue);

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndVertical();

                        loadedVulnerableCard.CounterEffects = counters;
                    }

                    #endregion
                }
            }
            #endregion

            #region drawing attacks
            if (loadedAttackerCard != null) {
                bool bothAttackType = false;
                showAttacks = EditorGUILayout.Foldout(showAttacks, "Attacks");
                if (showAttacks) {
                    GUILayout.Label("Attacks");

                    if (loadedOrganicAttackerCard != null) {
                        loadedOrganicAttackerCard.AttackType = (AttackTypes)EditorGUILayout.EnumPopup("Damage type:", loadedOrganicAttackerCard.AttackType);

                        if (loadedOrganicAttackerCard.AttackType != currentAttackType) {
                            if (loadedOrganicAttackerCard.AttackType == AttackTypes.Both) {
                                // unfold skill side. Because AttackTypes.Both will have 2 different skill effects depend of the attack type.
                                showSkill = true;
                                currentAttackType = loadedOrganicAttackerCard.AttackType;
                            }
                        }

                        if (loadedOrganicAttackerCard.AttackType == AttackTypes.Both) {
                            bothAttackType = true;
                        }
                    }

                    GUILayout.Space(10);

                    var attacks = fixEffectsArray(loadedAttackerCard.Attacks);

                    EditorGUILayout.BeginVertical();

                    for (int i = 0, length = attacks.Length; i < length; i++) {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label(attacks[i].EffectType.ToString(), EditorStyles.helpBox, GUILayout.Width(100));

                        int power;
                        var powerAsString = attacks[i].Power.ToString();
                        powerAsString = GUILayout.TextField(powerAsString, GUILayout.Width(30));
                        if (int.TryParse(powerAsString, out power))
                            attacks[i].Power = Mathf.Clamp(power, 0, int.MaxValue);

                        if (bothAttackType) {
                            GUILayout.Label("Ranged: ", EditorStyles.helpBox, GUILayout.Width(60));

                            int rangedPower;
                            var rangedPowerAsString = attacks[i].RangedPower.ToString();
                            rangedPowerAsString = GUILayout.TextField(rangedPowerAsString, GUILayout.Width(30));
                            if (int.TryParse(rangedPowerAsString, out rangedPower))
                                attacks[i].RangedPower = Mathf.Clamp(rangedPower, 0, int.MaxValue);
                        }

                        EditorGUILayout.EndHorizontal();
                    }


                    EditorGUILayout.EndVertical();

                    loadedAttackerCard.Attacks = attacks;
                }
            }
            #endregion

            #region drawing healer
            if (loadedHealerCard != null) {
                showHealer = EditorGUILayout.Foldout(showHealer, "Healer");
                if (showHealer) {
                    int heal;
                    string healAsText = loadedHealerCard.Heal.ToString();
                    if (int.TryParse(EditorGUIHelper.DrawTextField("Heal:", ref healAsText), out heal)) {
                        loadedHealerCard.Heal = heal;
                    }
                }
            }
            #endregion

            GUILayout.EndScrollView();

            // draw card avatar.
            var windowSize = myWindow.position.size;
            Rect targetRect = new Rect(windowSize.x - 85, 70, 75, 120);
            if (CurrentTexture != null) {
                GUI.DrawTexture(targetRect, CurrentTexture.GetPreview ());
            }

            targetRect.position -= Vector2.one * 2;
            targetRect.size += Vector2.one * 4;
            GUI.DrawTexture(targetRect, EasyCardEditor.CardCover);

            if (GUI.Button(new Rect(windowSize.x - 85, 195, 75, 20), "change")) {
                Images.Show("Size: 250x400", "Avatars", (id, image) => {
                    loadedCard.Avatar = id;
                    CurrentTexture = image;
                    myWindow.Focus();
                });
            }
            //

            void savecard<T>(T data) where T : BaseCard {
                var dat = JsonUtility.ToJson(data, true);
                var path = EasyCardEditor.AssetsPath + "/Cards/" + cardName + ".json";
                File.WriteAllText(path, dat);

                CardDropperTesting.Clear();
            }

            void preview<T>(T card) where T : BaseCard {
                CardEditorPreview.Preview(JsonUtility.ToJson(card));
            }

            void createCard(bool saveOrPreview = false) {
                switch (loadedCard.CardInteractionType) {
                    case CardInteractionTypes.Organic:
                        var organicCard = new OrganicCard();
                        organicCard.Health = loadedVulnerableCard.Health;
                        organicCard.Points = loadedVulnerableCard.Points;
                        organicCard.Resistances = loadedVulnerableCard.Resistances;
                        organicCard.CounterEffectParticle = loadedVulnerableCard.CounterEffectParticle;
                        organicCard.CounterEffects = loadedVulnerableCard.CounterEffects;
                        organicCard.CounterEnabled = loadedVulnerableCard.CounterEnabled;

                        switch (loadedCard.CardType) {
                            case CardTypes.Attacker:
                                var attackCard = new OrganicAttackerCard();
                                attackCard.SetBase(loadedCard);
                                attackCard.AttackType = loadedOrganicAttackerCard.AttackType;
                                attackCard.Attacks = loadedAttackerCard.Attacks;

                                attackCard.RangedEffectParticle = loadedOrganicAttackerCard.RangedEffectParticle;
                                attackCard.RangedEffectRange = loadedOrganicAttackerCard.RangedEffectRange;
                                attackCard.RangedEffectBothRows = loadedOrganicAttackerCard.RangedEffectBothRows;

                                attackCard.SetOrganicCard(organicCard);

                                if (!saveOrPreview) {
                                    savecard(attackCard);
                                } else {
                                    preview(attackCard);
                                }

                                break;

                            case CardTypes.Healer:
                                var healCard = new OrganicHealerCard();
                                healCard.SetBase(loadedCard);
                                healCard.Heal = loadedHealerCard.Heal;

                                healCard.SetOrganicCard(organicCard);

                                if (!saveOrPreview) {
                                    savecard(healCard);
                                } else {
                                    preview(healCard);
                                }

                                break;
                        }

                        break;

                    case CardInteractionTypes.Skill:
                        switch (loadedCard.CardType) {
                            case CardTypes.Attacker:
                                var attackCard = new SkillAttackerCard();
                                attackCard.SetBase(loadedCard);
                                attackCard.Attacks = loadedAttackerCard.Attacks;

                                if (!saveOrPreview) {
                                    savecard(attackCard);
                                } else {
                                    preview(attackCard);
                                }
                                break;

                            case CardTypes.Healer:
                                var healCard = new SkillHealerCard();
                                healCard.SetBase(loadedCard);
                                healCard.Heal = loadedHealerCard.Heal;

                                if (!saveOrPreview) {
                                    savecard(healCard);
                                } else {
                                    preview(healCard);
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
