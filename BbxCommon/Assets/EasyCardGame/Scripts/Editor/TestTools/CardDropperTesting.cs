using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

using CardGame.Editor;
using CardGame.GameData.Cards;
using CardGame.Loaders;

using System.Linq;

public class CardDropperTesting : EditorWindow {
    [MenuItem("Easy Card Game/Test Tools/Card Drop Test")]
    public static void Init() {
        isDescending = true;
        lastOrder = false;

        var window = (CardDropperTesting)GetWindow(typeof(CardDropperTesting));
        window.Show();
        window.titleContent = new GUIContent ("Card Drop Test");
    }

    public static void Clear () {
        List = null;
    }

    private CardDropperTesting myWindow;
    private Vector2 scrollPos;

    private int howMany = 100;

    private static List<KeyValuePair<TextAsset, int[]>> List;

    private static bool lastOrder;
    private static bool isDescending;

    private float totalDropRate;
    private int droppedCount;

    void reOrder (int i) {
        if (isDescending) {
            var sortedDict = from entry in List orderby entry.Value[i] descending select entry;
            List = sortedDict.ToList();
        } else {
            var sortedDict = from entry in List orderby entry.Value[i] ascending select entry;
            List = sortedDict.ToList();
        }
    }

    void OnGUI() {
        if (List == null) {
            List = new List<KeyValuePair<TextAsset, int[]>>();
        }

        if (myWindow == null) {
            myWindow = (CardDropperTesting)GetWindow(typeof(CardDropperTesting));
        }

        GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);

        GUI.skin = EasyCardEditor.GuiSkin;

        GUILayout.Label("Drop some cards to check the drop rates and rarities.");

        GUILayout.Space(10);

        string cardCount = GUILayout.TextField(howMany.ToString());
        if (int.TryParse (cardCount, out int value)) {
            howMany = value;
        }

        if (GUILayout.Button(string.Format ("Drop {0} Cards", howMany.ToString ()))) {
            Dictionary<TextAsset, int[]> counter = new Dictionary<TextAsset, int[]>();

            // create card randomizer.
            var cardPool = CreateInstance<Pool>();
            cardPool.LoadFolderWithoutInstantiate<TextAsset>("Cards");
            var cardRandomizer = new Randomizer<TextAsset>(cardPool.Count);

            var loadedCards = cardPool.GetAllObjects();
            int length = loadedCards.Length;

            totalDropRate = 0;

            for (int i = 0; i < length; i++) {
                var textAsset = (TextAsset)loadedCards[i].Get();
                int dropRate = JsonUtility.FromJson<BaseCard>(textAsset.text).DropRate;
                totalDropRate += dropRate;
                counter.Add(textAsset, new int[2] { dropRate, 0 });
                cardRandomizer.AddMember(textAsset, dropRate);
            }

            for (int i=0; i<howMany; i++) {
                TextAsset card = cardRandomizer.Select();
                counter[card][1]++;
            }

            List = counter.ToList();

            reOrder(!lastOrder ? 1: 0);
        }

        if (List.Count == 0) {
            return;
        }

        GUILayout.Space(10);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Card file name", GUILayout.Width(180));

        if (lastOrder) {
            GUI.color = Color.yellow;
        }

        if (GUILayout.Button ("Drop rate",GUILayout.Width(100))) {
            if (!lastOrder) {
                lastOrder = true;
                isDescending = true;
            } else {
                isDescending = !isDescending;
            }

            reOrder(0);
        }

        if (!lastOrder) {
            GUI.color = Color.yellow;
        } else {
            GUI.color = Color.white;
        }

        if (GUILayout.Button("Dropped", GUILayout.Width(70))) {
            if (lastOrder) {
                lastOrder = false;
                isDescending = true;
            } else {
                isDescending = !isDescending;
            }

            reOrder(1);
        }

        GUI.color = Color.white;

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        int index = 0;
        int count = List.Count;
 
        foreach (var c in List) {
            GUILayout.BeginHorizontal();

            GUILayout.Label((!isDescending ? (count - index) : (index + 1)).ToString(),  GUILayout.Width(20));

            if (GUILayout.Button (c.Key.name, GUILayout.Width (160))) {
                ShowCardEditor.Init(c.Key.text, c.Key.name, EasyCardEditor.SkillEffects);
            }

            GUILayout.Label(string.Format ("%{0} ({1})", System.Math.Round (c.Value[0] / totalDropRate * 100f, 2),c.Value[0].ToString()), GUILayout.Width(100));
            GUILayout.Label(c.Value[1].ToString(), GUILayout.Width(70));

            GUILayout.EndHorizontal();

            index++;
        }
        
        GUILayout.EndVertical();
        
        GUILayout.EndScrollView();
    }
}
