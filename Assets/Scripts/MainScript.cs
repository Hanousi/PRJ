using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

// Show off all the Debug UI components.
public class MainScript : MonoBehaviour
{
    public GameObject gameAssets;
    public GameObject noteCatcher;
    public GameObject canvasWithDebug;
    public Transform hiHatNote;
    public Transform crashNote;
    public Transform snareDrumNote;
    public Transform hiTomNote;
    public Transform midTomNote;
    public Transform floorTomNote;
    public Transform rideNote;
    private string dataPath;
    private GameObject currentUI;
    private bool inGame = false;
    private float timeLeft = -1;
    private float[][] currentLevel;
    private Dictionary<int, GameLevel> levels;
    private GamePerformance currentPerformance;
    private PerformanceRecord performanceRecord;
    private Dictionary<string, int> ghostHits = new Dictionary<string, int>()
    {
        { "HiHat", 0 },
        { "Crash", 0 },
        { "SnareDrum", 0 },
        { "HiTom", 0 },
        { "MidTom", 0},
        { "FloorTom", 0},
        { "Ride", 0 }
    };
    bool inMenu;

    void Start()
    {
        LevelSetUp();
        dataPath = Path.Combine(Application.persistentDataPath, "PerformanceData.dat");
        performanceRecord = loadPerformanceData(dataPath);

        foreach (GamePerformance gp in performanceRecord.GetQueue())
        {
            Debug.Log(gp.ToString());
        }

        Debug.Log(performanceRecord.GetQueue().Count);

        //StartGame(3);
        buildMainMenu();

        inMenu = false;
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
            
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            AudioSource kick = GetComponent<AudioSource>();
            kick.Play(0);
        }

        if(inGame)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft < 0)
            {
                inGame = false;
                GamePerformance gamePerformance = new GamePerformance(0, 0, 0, 0, 0, 0, 0);

                NoteCatcherController noteCatcherController = noteCatcher.GetComponent<NoteCatcherController>();
                Dictionary<string, string> results = getGameResults(noteCatcherController.missedNotes);

                savePerformanceData();

                Destroy(currentUI);
                Destroy(GameObject.Find("UIHelpers(Clone)"));

                currentUI = Instantiate(canvasWithDebug, new Vector3(0.32f, -0.46f, 2.43f), Quaternion.identity);

                DebugUIBuilder.instance.AddLabel("Missed notes: ");
                foreach (KeyValuePair<string, string> kvp in results)
                {
                    DebugUIBuilder.instance.AddLabel(kvp.Key + ": " + kvp.Value);
                }

                DebugUIBuilder.instance.AddLabel("Missed hits: ", DebugUIBuilder.DEBUG_PANE_RIGHT);
                foreach (KeyValuePair<string, int> kvp in ghostHits)
                {
                    DebugUIBuilder.instance.AddLabel(kvp.Key + ": " + kvp.Value, DebugUIBuilder.DEBUG_PANE_RIGHT);
                }

                DebugUIBuilder.instance.AddButton("Close", enterSandBoxMode);
                DebugUIBuilder.instance.Show();

                resetGhostHits();
                noteCatcherController.resetResults();
            }
        }
    }

    void OnEnable()
    {
        DrumstickController.OnMiss += AddMiss;    
    }

    void OnDisable()
    {
        DrumstickController.OnMiss -= AddMiss;    
    }

    private void AddMiss(string noteName)
    {
        ghostHits[noteName] = ghostHits[noteName] + 1;
    }

    private void enterSandBoxMode()
    {
        Destroy(currentUI);
        Destroy(GameObject.Find("UIHelpers(Clone)"));
        buildMainMenu();

        gameAssets.SetActive(false);
        DebugUIBuilder.instance.Hide();
        inMenu = false;
    }

    private Dictionary<string, string> getGameResults(Dictionary<string, int> missedNotes)
    {
        Dictionary<string, string> results = new Dictionary<string, string>();

        for (int i = 0; i < currentLevel.Length; i++)
        {
            switch (i)
            {
                case 0:
                    getHitRate("HiHatNote", currentLevel[i].Length, missedNotes, results);
                    
                    break;
                case 1:
                    getHitRate("CrashNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 2:
                    getHitRate("SnareDrumNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 3:
                    getHitRate("HiTomNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 4:
                    getHitRate("MidTomNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 5:
                    getHitRate("FloorTomNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 6:
                    getHitRate("RideNote", currentLevel[i].Length, missedNotes, results);

                    break;
            }
        }

        return results;
    }

    private void LevelSetUp()
    {
        levels = new Dictionary<int, GameLevel>();

        GameLevel game1 = new GameLevel(1, new float[][] { new float[] { 1, 2, 3, 4, 5, 5.5f, 6, 6.5f, 7, 7.5f, 8, 8.5f,
                9, 10, 11, 12, 13, 13.5f, 14, 14.5f, 15, 15.5f, 16, 16.5f,
                17, 18, 19, 20, 21, 21.5f, 22, 22.5f, 23, 23.5f, 24, 24.5f },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { }},
            18,
            new string[] { "hiHat" });

        GameLevel game2 = new GameLevel(2, new float[][] { new float[] { 1, 2, 3, 4, 5, 5.5f, 6, 6.5f, 7, 7.5f, 8, 8.5f },
            new float[] { 9, 10, 11, 12, 13, 13.5f, 14, 14.5f, 15, 15.5f, 16, 16.5f },
            new float[] { 17, 18, 19, 20 },
            new float[] { 21, 21.5f, 22, 22.5f, 23, 23.5f, 24, 24.5f },
            new float[] { 25, 26, 29, 29.5f, 30, 30.5f },
            new float[] { 27, 28, 31, 31.5f, 32, 32.5f },
            new float[] {  }},
            23,
            new string[] { });

        GameLevel game3 = new GameLevel(3, new float[][] { new float[] { },
            new float[] { },
            new float[] { 3, 7, 11, 15, 19, 23, 27 },
            new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 },
            new float[] { },
            new float[] { },
            new float[] { }},
            20,
            new string[] { "hiHat", "snareDrum" });

        GameLevel game4 = new GameLevel(4, new float[][] { new float[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 25.5f, 26.5f },
            new float[] { },
            new float[] { 3, 7, 11, 15, 19, 23, 26 },
            new float[] { },
            new float[] { },
            new float[] { 1, 5, 9, 13, 17, 21, 25 },
            new float[] { }},
            20,
            new string[] { "hiHat", "snareDrum", "floorTom" });

        GameLevel game5 = new GameLevel(5, new float[][] { new float[] { },
            new float[] { },
            new float[] { },
            new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 },
            new float[] { },
            new float[] { },
            new float[] { } },
            20,
            new string[] { "hiHat", "snareDrum", "floorTom" });

        levels.Add(1, game1);
        levels.Add(2, game2);
        levels.Add(3, game3);
        levels.Add(4, game4);
        levels.Add(5, game5);
    }

    void StartGame(int level)
    {
        GameLevel gameLevel = levels[level];
        float[][] notePositions = gameLevel.GetNotePositions();
        currentLevel = notePositions;
        currentPerformance = new GamePerformance(0, 0, 0, 0, 0, 0, 0);

        DebugUIBuilder.instance.Hide();
        for(int i = 0; i < notePositions.Length; i++)
        {
            float[] drumNotePositions = notePositions[i];
            //private void createNote(Transform note, float[] drumNotePositions, float x, float xOffset, float xModifier, float y, float yOffset, float yModifier, float zOffset)

            switch (i)
            {
                case 0:
                    createNote(hiHatNote, drumNotePositions, 0, -0.59f, -0.106f, 0.8f, 0, 0, 0);

                    break;
                case 1:
                    createNote(crashNote, drumNotePositions, 0, -0.18f, -0.106f, 1.12f, 0, 0, 0.92f);

                    break;
                case 2:
                    createNote(snareDrumNote, drumNotePositions, -0.07f, 0, 0, 0.6f, 0, 0, 0.28f);

                    break;
                case 3:
                    createNote(hiTomNote, drumNotePositions, 0.05f, 0, 0, 0, 0.85f, 0.18f, 0.95f);

                    break;
                case 4:
                    createNote(midTomNote, drumNotePositions, 0.47f, 0, 0, 0, 0.85f, 0.18f, 0.95f);

                    break;
                case 5:
                    createNote(floorTomNote, drumNotePositions, 0.63f, 0, 0, 0.6f, 0, 0, 0.28f);

                    break;
                case 6:
                    createNote(rideNote, drumNotePositions, 0, 0.87f, 0.106f, 1, 0, 0, 0.6f);

                    break;
            }
        }

        timeLeft = gameLevel.GetDuration();
        gameAssets.SetActive(true);
        inGame = true;

        inMenu = false;
    }

    private void buildMainMenu()
    {
        currentUI = Instantiate(canvasWithDebug, new Vector3(0.32f, -0.46f, 2.43f), Quaternion.identity);

        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddButton("Level 1", delegate () { StartGame(1); });
        DebugUIBuilder.instance.AddButton("Level 2", delegate () { StartGame(2); });
        DebugUIBuilder.instance.AddButton("Level 3", delegate () { StartGame(3); });
        DebugUIBuilder.instance.AddButton("Level 4", delegate () { StartGame(4); });
        DebugUIBuilder.instance.AddButton("Level 5", delegate () { StartGame(5); });
        DebugUIBuilder.instance.AddDivider();

        if (performanceRecord.GetQueue().Count == 5)
        {
            GameLevel aiRecommendation = getRecommendation();

            if (aiRecommendation != null)
            {
                int levelNumber = aiRecommendation.GetLevelNumber();
                string[] tags = aiRecommendation.GetTags();
                string tagList = "";

                for (int i = 0; i < tags.Length - 1; i++)
                {
                    tagList = tagList + tags[i] + ", ";
                }

                if (tagList == "")
                {
                    tagList = tags[0];
                } else
                {
                    tagList = tagList + "and " + tags[tags.Length - 1];
                }

                Debug.Log(levelNumber);

                DebugUIBuilder.instance.AddLabel("AI Suggesstion:", DebugUIBuilder.DEBUG_PANE_RIGHT);
                DebugUIBuilder.instance.AddLabel("It looks like your are struggling with playing the " + tagList + ". Try this exercise out:", DebugUIBuilder.DEBUG_PANE_RIGHT);

                DebugUIBuilder.instance.AddButton("Level " + levelNumber, delegate () { StartGame(levelNumber); }, DebugUIBuilder.DEBUG_PANE_RIGHT);
            }
        }
    }

    private GameLevel getRecommendation()
    {
        Queue<GamePerformance> queue = performanceRecord.GetQueue();

        GamePerformance averageScores = queue.Aggregate((x, y) => new GamePerformance(
            x.hiHat + y.hiHat,
            x.crash + y.crash,
            x.snareDrum + y.snareDrum,
            x.hiTom + y.hiTom,
            x.midTom + y.midTom,
            x.floorTom + y.floorTom,
            x.ride + y.ride
        ));

        averageScores.averageScores(queue.Count);

        string tag = averageScores.getMaxScore();
        Debug.Log(tag);
        foreach(KeyValuePair<int, GameLevel> kvp in levels)
        {
            GameLevel level = kvp.Value;

            if(Array.IndexOf(level.GetTags(), tag) > -1)
            {
                return level;
            }
        }

        return null;
    }

    private void createNote(Transform note, float[] drumNotePositions, float x, float xOffset, float xModifier, float y, float yOffset, float yModifier, float zOffset)
    {
        foreach (float position in drumNotePositions)
        {
            Transform gameObject = Instantiate(note, new Vector3(
                x == 0 ? (float)(xOffset + (position * xModifier)) : x,
                y == 0 ? (float)(yOffset + (position * yModifier)) : y,
                (float)((position * 2) + zOffset)
                ), Quaternion.identity);

            NoteController noteController = gameObject.GetComponent<NoteController>();
            string drumName = Regex.Split(note.name, "Note")[0];
            string targetName =  drumName + "Target";
            string speedName = drumName + "Speed";

            noteController.SetTarget(GameObject.Find(targetName).transform);
            noteController.SetSpeed((float)GetConstantField(speedName));
        }
    }

    private void getHitRate(string noteName, int totalNotes, Dictionary<string, int> missedNotes, Dictionary<string, string> results)
    {
        if (totalNotes == 0)
        {
            results.Add(noteName, "100%");
        }
        else
        {
            double hitRate = (totalNotes - missedNotes[noteName]) / (double)totalNotes;
            string drumName = LowercaseFirst(Regex.Split(noteName, "Note")[0]);

            if (hitRate < 0.7) {
                SetFieldValue(currentPerformance, drumName, 3);
            } else if(hitRate < 0.8) {
                SetFieldValue(currentPerformance, drumName, 2);
            } else if(hitRate < 0.9) {
                SetFieldValue(currentPerformance, drumName, 1);
            }

            results.Add(noteName, (hitRate * 100).ToString("0.##") + "%");
        }
    }

    private void savePerformanceData()
    {
        getGhostHitScores();
        performanceRecord.Add(currentPerformance);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, performanceRecord);
        }
    }

    private static PerformanceRecord loadPerformanceData(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        try
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                return (PerformanceRecord)binaryFormatter.Deserialize(fileStream);
            }
        } catch (FileNotFoundException e)
        {
            Debug.Log("Error: File not found, creating new load file.");

            return new PerformanceRecord();
        }

    }

    private void getGhostHitScores()
    {
        foreach(KeyValuePair<string, int> kvp in ghostHits)
        {
            string drumName = LowercaseFirst(Regex.Split(kvp.Key, "Note")[0]);
            float currentScore = (float)GetFieldValue(currentPerformance, drumName);

            if (kvp.Value > 20) {
                SetFieldValue(currentPerformance, drumName, currentScore + 3);
            } else if(kvp.Value > 15) {
                SetFieldValue(currentPerformance, drumName, currentScore + 2);
            } else if (kvp.Value > 10) {
                SetFieldValue(currentPerformance, drumName, currentScore + 1);
            }
        }
    }

    private void resetGhostHits()
    {
        List<string> keys = new List<string>(ghostHits.Keys);

        foreach (string key in keys)
        {
            ghostHits[key] = 0;
        }
    }

    public static void SetFieldValue(object instance, string strPropertyName, object newValue)
    {  
        Type type = instance.GetType();
        FieldInfo fieldInfo = type.GetField(strPropertyName);

        fieldInfo.SetValue(instance, newValue);
    }

    public static object GetFieldValue(object instance, string strPropertyName)
    {
        Type type = instance.GetType();
        FieldInfo fieldInfo = type.GetField(strPropertyName);

        return fieldInfo.GetValue(instance);
    }

    public static object GetConstantField(string strPropertyName)
    {
        Type type = typeof(Constants);
        FieldInfo fieldInfo = type.GetField(strPropertyName);

        return fieldInfo.GetValue(null);
    }

    static string LowercaseFirst(string s)
    {
        return char.ToLower(s[0]) + s.Substring(1);
    }
}
