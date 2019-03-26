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

/// <summary>
/// Class holds all the commands and properties required to run the application in it's main thread.
/// Attached to the MainScript GameObject in the Main scene, the script's biggest reposibilities revolve around
/// game level creation and UI managment as well as taking advantage of other helper classes to achieve the hueristic based AI.
/// </summary>
public class MainScript : MonoBehaviour
{
    public GameObject gameAssets;
    public GameObject nonGameAssets;
    public GameObject noteCatcher;
    /// <summary>
    /// These variables are used to handle the canvasWithDebug object provided with the Oculus integration package in a manner
    /// which allows it to tear down and create multiple UI's as the user navigates through the application.
    /// </summary>
    private GameObject currentUI;
    public GameObject canvasWithDebug;
    /// <summary>
    /// Transform objects attached in the Unity UI to be able to generate note objects with the correct
    /// colour and positioning dynamically
    /// </summary>
    public Transform hiHatNote;
    public Transform crashNote;
    public Transform snareDrumNote;
    public Transform hiTomNote;
    public Transform midTomNote;
    public Transform floorTomNote;
    public Transform rideNote;
    private string dataPath;
    /// <summary>
    /// Variables used only by the Update function to effectively set a timer to handle when a specific game level finishes.
    /// </summary>
    private bool inGame = false;
    private float timeLeft = -1;
    private float[][] currentLevel;
    /// <summary>
    /// Collections used to store the levels the game has available as well as the rhythm templates the AI is able to choose from.
    /// </summary>
    private Dictionary<int, GameLevel> levels;
    private List<AILevelTemplate> levelTemplates = new List<AILevelTemplate>() { };
    /// <summary>
    /// Instances of helper classes used to track the performance of the user on previous exercises they have played for both the
    /// saving of data and usage by the AI.
    /// </summary>
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

    /// <summary>
    /// Start function called as soon as the application is started up. Mainly focuses on the load of data, more specifically
    /// performance data from previous sessions of using the application as well as level creation for both the exercises and
    /// AI level templates.
    /// </summary>
    void Start()
    {
        LevelSetUp();
        dataPath = Path.Combine(Application.persistentDataPath, "PerformanceData.dat");
        performanceRecord = LoadPerformanceData(dataPath);

        //StartGame(3);
        BuildMainMenu();

        inMenu = false;
    }

    /// <summary>
    /// Update function called with every frame produced in the application. Used to check for inputs from
    /// the user as well as handling actions required for when a exercise has finished.
    /// </summary>
    void Update()
    {            
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            AudioSource kick = GetComponent<AudioSource>();
            kick.Play(0);
        }

        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            Debug.Log("Hani");
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }

        if (inGame)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft < 0)
            {
                inGame = false;

                NoteCatcherController noteCatcherController = noteCatcher.GetComponent<NoteCatcherController>();
                Dictionary<string, string> results = GetGameResults(noteCatcherController.missedNotes);

                SavePerformanceData();
                DestroyCurrentUI();

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

                DebugUIBuilder.instance.AddButton("Close", EnterSandBoxMode);
                DebugUIBuilder.instance.Show();

                ResetGhostHits();
                noteCatcherController.resetResults();
            }
        }
    }

    /// <summary>
    /// Function called when the object becomes enabled and active. Used in this script to attach the Add miss function
    /// to the OnMiss event found on the Drumstick controller script.
    /// </summary>
    void OnEnable()
    {
        DrumstickController.OnMiss += AddMiss;    
    }

    /// <summary>
    /// Function called when the object becomes disabled. This is done so that the AddMiss function is no longer called
    /// when the event is triggered.
    /// </summary>
    void OnDisable()
    {
        DrumstickController.OnMiss -= AddMiss;    
    }

    private void AddMiss(string noteName)
    {
        if (inGame)
        {
            ghostHits[noteName] = ghostHits[noteName] + 1;
        }
    }

    private void DestroyCurrentUI()
    {
        Destroy(currentUI);
        // UIHelper Instance generated by the previous canvasWithDebug UI must also be deleted so that the laserPointer on the
        // new UI is still functional. 
        Destroy(GameObject.Find("UIHelpers(Clone)"));
    }

    /// <summary>
    /// When an exercise is finished the application always returns back to sandbox mode. Function handles the UI and scene changes
    /// required to achieve.
    /// </summary>
    private void EnterSandBoxMode()
    {
        DestroyCurrentUI();
        BuildMainMenu();

        gameAssets.SetActive(false);
        nonGameAssets.SetActive(true);
        inMenu = false;
    }

    /// <summary>
    /// During an exercise the notes the user doesn't manage to hit are caught by the noteCatcherController instance.
    /// Using the data collected by that class the function calculates the hitrate achieved by the user on every drum
    /// in the exercise in an attempt to find the drum with the lowest hitrate.
    /// </summary>
    /// <param name="missedNotes">Dictionary with a string key representing the name of the drum and a int value 
    /// recording the notes captured that belonged to that drum</param>
    /// <returns>Dictionary with a string key representing the name of the drum note missed and a string value of the
    /// percentage hitrate for the drum in a prettyfied format for UI usage</returns>
    private Dictionary<string, string> GetGameResults(Dictionary<string, int> missedNotes)
    {
        Dictionary<string, string> results = new Dictionary<string, string>();

        for (int i = 0; i < currentLevel.Length; i++)
        {
            switch (i)
            {
                case 0:
                    GetHitRate("HiHatNote", currentLevel[i].Length, missedNotes, results);
                    
                    break;
                case 1:
                    GetHitRate("CrashNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 2:
                    GetHitRate("SnareDrumNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 3:
                    GetHitRate("HiTomNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 4:
                    GetHitRate("MidTomNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 5:
                    GetHitRate("FloorTomNote", currentLevel[i].Length, missedNotes, results);

                    break;
                case 6:
                    GetHitRate("RideNote", currentLevel[i].Length, missedNotes, results);

                    break;
            }
        }

        GetGhostHitHeuristics();

        return results;
    }

    /// <summary>
    /// Function populates collection variables required to populate the game scene when the exercises are ran.
    /// </summary>
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
            new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 },
            new float[] { 3, 7, 11},
            new float[] { },
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
            new float[] { 3, 7, 11, 15, 19, 23, 27 },
            new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 },
            new float[] { },
            new float[] { },
            new float[] { }},
            20,
            new string[] { "hiHat", "snareDrum", "floorTom" });

        AILevelTemplate template1 = new AILevelTemplate(new float[][] { Array.ConvertAll(Enumerable.Range(1, 28).ToArray(), x => (float)x), new float[] { 3, 7, 11, 15, 19, 23, 27 } }, 20);
        AILevelTemplate template2 = new AILevelTemplate(new float[][] {
            new float[] { 1, 2.5f, 4, 6.5f, 8, 9, 10.5f, 12, 14.5f, 16, 17, 18.5f, 20, 22.5f, 24 },
            new float[] { 3, 7, 11, 15, 19, 23 }
        }, 20);

        levels.Add(1, game1);
        levels.Add(2, game2);
        levels.Add(3, game3);
        levels.Add(4, game4);
        levels.Add(5, game5);

        levelTemplates.Add(template1);
        levelTemplates.Add(template2);
    }

    /// <summary>
    /// Handles the scene for when a exercise is initiated by setting the exercise timer, placing note gameobjects in the correct location
    /// and preparing the objects required to capture the user performance.
    /// </summary>
    /// <param name="level">The exercise level number that is selected by the user. Used as the key to retrieve the Gamelevel object for 
    /// the note positions</param>
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

            switch (i)
            {
                case 0:
                    CreateNote(hiHatNote, drumNotePositions, 0, -0.59f, -0.106f, 0.8f, 0, 0, 0);

                    break;
                case 1:
                    CreateNote(crashNote, drumNotePositions, 0, -0.18f, -0.106f, 1.12f, 0, 0, 0.92f);

                    break;
                case 2:
                    CreateNote(snareDrumNote, drumNotePositions, -0.07f, 0, 0, 0.6f, 0, 0, 0.28f);

                    break;
                case 3:
                    CreateNote(hiTomNote, drumNotePositions, 0.05f, 0, 0, 0, 0.85f, 0.18f, 0.95f);

                    break;
                case 4:
                    CreateNote(midTomNote, drumNotePositions, 0.47f, 0, 0, 0, 0.85f, 0.18f, 0.95f);

                    break;
                case 5:
                    CreateNote(floorTomNote, drumNotePositions, 0.63f, 0, 0, 0.6f, 0, 0, 0.28f);

                    break;
                case 6:
                    CreateNote(rideNote, drumNotePositions, 0, 0.87f, 0.106f, 1, 0, 0, 0.6f);

                    break;
            }
        }

        timeLeft = gameLevel.GetDuration();
        gameAssets.SetActive(true);
        nonGameAssets.SetActive(false);
        inGame = true;

        inMenu = false;
    }

    private void BuildMainMenu()
    {
        currentUI = Instantiate(canvasWithDebug, new Vector3(-0.5f, 0f, 3.5f), Quaternion.identity);

        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddButton("Level 1", delegate () { StartGame(1); });
        DebugUIBuilder.instance.AddButton("Level 2", delegate () { StartGame(2); });
        DebugUIBuilder.instance.AddButton("Level 3", delegate () { StartGame(3); });
        DebugUIBuilder.instance.AddButton("Level 4", delegate () { StartGame(4); });
        DebugUIBuilder.instance.AddButton("Level 5", delegate () { StartGame(5); });
        DebugUIBuilder.instance.AddDivider();

        if (performanceRecord.GetQueue().Count == Constants.PERFORMANCERECORDSIZE)
        {
            GameLevel aiRecommendation = GetRecommendation();

            if (aiRecommendation != null)
            {
                int levelNumber = aiRecommendation.GetLevelNumber();
                string[] tags = aiRecommendation.GetTags();
                string aiText = getAIText(tags);

                DebugUIBuilder.instance.AddLabel("AI Suggesstion:", DebugUIBuilder.DEBUG_PANE_RIGHT);
                DebugUIBuilder.instance.AddLabel("It looks like your are struggling with playing the " + aiText + ". Try this exercise out:", DebugUIBuilder.DEBUG_PANE_RIGHT);
                DebugUIBuilder.instance.AddButton("Level " + levelNumber, delegate () { StartGame(levelNumber); }, DebugUIBuilder.DEBUG_PANE_RIGHT);

                DebugUIBuilder.instance.AddDivider(DebugUIBuilder.DEBUG_PANE_RIGHT);

                DebugUIBuilder.instance.AddLabel("AI Level:", DebugUIBuilder.DEBUG_PANE_RIGHT);
                DebugUIBuilder.instance.AddLabel("Have a go at this AI developed level to help improve your weaker skillset!", DebugUIBuilder.DEBUG_PANE_RIGHT);
                DebugUIBuilder.instance.AddButton("Level AI", delegate () { StartGame(-1); }, DebugUIBuilder.DEBUG_PANE_RIGHT);
            }
        }
    }

    /// <summary>
    /// Calculates the user's least performant drum and then selects a GameLevel which includes that drum
    /// </summary>
    /// <returns>GameLevel which involves that drum which is made as a suggestion to the user in the main menu</returns>
    private GameLevel GetRecommendation()
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

        averageScores.averageScores();

        string tag = averageScores.getMaxScore();
        Debug.Log(tag);
        CreateAILevel(tag);

        foreach (KeyValuePair<int, GameLevel> kvp in levels)
        {
            GameLevel level = kvp.Value;

            if(Array.IndexOf(level.GetTags(), tag) > -1)
            {
                return level;
            }
        }

        return null;
    }

    /// <summary>
    /// Builds a dynamic exercise level around the drum that is selected in the AIRecommendation function.
    /// A exercise template is chosen at random and accompanying drums to the template are also selected at random
    /// around the focus of the exercise being the drum provided as a parameter.
    /// </summary>
    /// <param name="tag">The drum name of the weakest drum selected by the AIRecommendation function</param>
    private void CreateAILevel(string tag)
    {
        System.Random rand = new System.Random();
        int weakDrumIndex = Constants.drumKey(tag);

        List<int> numbers = Enumerable.Range(0, 7).Where(u => u != weakDrumIndex).ToList();

        AILevelTemplate template = levelTemplates[rand.Next(levelTemplates.Count)];
        float[][] templatePositions = template.GetNoteTemplates();

        float[][] notePositions = new float[][] { new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { }
        };

        notePositions[weakDrumIndex] = templatePositions[0];

        for (int i = 1; i < templatePositions.Length; i++)
        {
            int drumCompanion = numbers[rand.Next(numbers.Count)];
            numbers.Remove(drumCompanion);
            notePositions[drumCompanion] = templatePositions[i];
        }

        GameLevel aiLevel = new GameLevel(-1, notePositions, template.GetDuration(), new string[] { });
        levels[-1] = aiLevel;
    }

    /// <summary>
    /// Instantiates all the notes in the scene that belong to one drum. Each set of drum notes requires a specific set of
    /// co-ordinates that it needs to follow due to the layout of the game board in the scene. Some notes will just move forward,
    /// others slightly to the right along the y axis and some move down the z axis. The set of parameters make sure that the 
    /// notes are aligned correctly in the scene.
    /// </summary>
    /// <param name="note">Transform object the scene will create. Linked to the script through the Unity UI</param>
    /// <param name="drumNotePositions">Float array containing the positions a specific note needs to be. In other words,
    /// these set numbers represent which beat in the song the note is present. e.i Bar 1 beat 3 = 3, Bar 4 beat 2 = 18</param>
    /// <param name="x">The position on the x axis the note needs to be instantied. If set to 0, the note must therefore
    /// move along this axis and the units provided by xOffset and xModifier are used instead.</param>
    /// <param name="xOffset">How far away from the scene's origin the notes need to be instantiated along the x axis</param>
    /// <param name="xModifier">The degree of change on this axis that must be reflected on the notes position depended on
    /// how far away it is from the origin.</param>
    /// <param name="y">The position on the y axis the note needs to be instantied. If set to 0, the note must therefore
    /// move along this axis and the units provided by yOffset and yModifier are used instead.</param>
    /// <param name="yOffset">How far away from the scene's origin the notes need to be instantiated along the y axis</param>
    /// <param name="yModifier">The degree of change on this axis that must be reflected on the notes position depended on
    /// how far away it is from the origin.</param>
    /// <param name="zOffset">How far away from the scene's origin the notes need to be instantiated along the z axis</param>
    private void CreateNote(Transform note, float[] drumNotePositions, float x, float xOffset, float xModifier, float y, float yOffset, float yModifier, float zOffset)
    {
        foreach (float position in drumNotePositions)
        {
            Transform gameObject = Instantiate(note, new Vector3(
                /* 
                 * If x or y is set to 0 then the position along this axis is not constant so instead the position offset and modifier
                 * are used instead. These parameters can be thought of as the x = mc + z linear line function where m is the xModifier
                 * and z is the xOffset. 
                 */
                x == 0 ? (float)(xOffset + (position * xModifier)) : x,
                y == 0 ? (float)(yOffset + (position * yModifier)) : y,
                (float)((position * 2) + zOffset)
                ), Quaternion.identity);

            NoteController noteController = gameObject.GetComponent<NoteController>();
            string drumName = Regex.Split(note.name, "Note")[0];
            string targetName =  drumName + "Target";
            string speedName = (drumName + "Speed").ToUpper();

            noteController.SetTarget(GameObject.Find(targetName).transform);
            noteController.SetSpeed((float)GetConstantField(speedName));
        }
    }

    /// <summary>
    /// Calculates the hitrate from the previous exercise for a given drum. Done by comparing the amount of notes present
    /// in the note positions array with the sum of numbers caught via the AddMiss event function.
    /// </summary>
    /// <param name="noteName">Name of the drum the hit rate is being calculated for</param>
    /// <param name="totalNotes">Number of notes that were present from the previous exercise</param>
    /// <param name="missedNotes">The collection which tracks the notes missed via the AddMiss event</param>
    /// <param name="results">Dictionary which contains every drum in the exercise as well as the prettified hit rate</param>
    private void GetHitRate(string noteName, int totalNotes, Dictionary<string, int> missedNotes, Dictionary<string, string> results)
    {
        if (totalNotes == 0)
        {
            results.Add(noteName, "100%");
        }
        else
        {
            double hitRate = (totalNotes - missedNotes[noteName]) / (double)totalNotes;
            string drumName = LowercaseFirst(Regex.Split(noteName, "Note")[0]);

            if (hitRate < Constants.HITRATEMAXTHRESHOLD) {
                SetFieldValue(currentPerformance, drumName, Constants.HITRATEMAXHEURISTIC);
            } else if(hitRate < Constants.HITRATEMIDTHRESHOLD) {
                SetFieldValue(currentPerformance, drumName, Constants.HITRATEMIDHEURISTIC);
            } else if(hitRate < Constants.HITRATEMINTHRESHOLD) {
                SetFieldValue(currentPerformance, drumName, Constants.HITRATEMINHEURISTIC);
            }

            results.Add(noteName, (hitRate * 100).ToString("0.##") + "%");
        }
    }

    /// <summary>
    /// Adds the most recent performance object to the record collection, converts to binary and saves it into a
    /// Persistent .dat file.
    /// </summary>
    private void SavePerformanceData()
    {
        performanceRecord.Add(currentPerformance);

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, performanceRecord);
        }
    }

    /// <summary>
    /// Attempts to load the Performance record file saved from the previous game. If not found a new Performance record
    /// collection is created instead.
    /// </summary>
    /// <param name="path"<>The file path which the persistent data file can be found</param>
    /// <returns>Returns a populated Performance collection if a file is found, otherwsie a empty one is returned</returns>
    private static PerformanceRecord LoadPerformanceData(string path)
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

    /// <summary>
    /// Converts the hitrates into hueristics which can be reasoned by the AI.
    /// </summary>
    private void GetGhostHitHeuristics()
    {
        foreach(KeyValuePair<string, int> kvp in ghostHits)
        {
            string drumName = LowercaseFirst(Regex.Split(kvp.Key, "Note")[0]);
            float currentScore = (float)GetFieldValue(currentPerformance, drumName);

            if (kvp.Value > Constants.GHOSTHITMAXTHRESHOLD) {
                SetFieldValue(currentPerformance, drumName, currentScore + Constants.GHOSTHITMAXHEURISTIC);
            } else if(kvp.Value > Constants.GHOSTHITMIDTHRESHOLD) {
                SetFieldValue(currentPerformance, drumName, currentScore + Constants.GHOSTHITMIDHEURISTIC);
            } else if (kvp.Value > Constants.GHOSTHITMINTHRESHOLD) {
                SetFieldValue(currentPerformance, drumName, currentScore + Constants.GHOSTHITMINHEURISTIC);
            }
        }
    }

    private void ResetGhostHits()
    {
        List<string> keys = new List<string>(ghostHits.Keys);

        foreach (string key in keys)
        {
            ghostHits[key] = 0;
        }
    }

    /// <summary>
    /// Helper function which takes the tags from a GameLevel and generates a comprehensible string list
    /// </summary>
    /// <param name="tags">String array of tags that need to be listed</param>
    /// <returns>Prettified string of tags in a list</returns>
    private string getAIText(string[] tags)
    {
        string tagList = "";

        for (int i = 0; i < tags.Length - 1; i++)
        {
            tagList = tagList + tags[i] + ", ";
        }

        if (tagList == "")
        {
            return tags[0];
        }
        else
        {
            return tagList + "and " + tags[tags.Length - 1];
        }
    }

    /// <summary>
    /// Helper function which allows to change the value of a field in a object at run time
    /// </summary>
    /// <param name="instance">Instance of the object you wish to change the field value of</param>
    /// <param name="strPropertyName">Name of the field you wish to change</param>
    /// <param name="newValue">The new value you wish to change the field value to</param>
    public static void SetFieldValue(object instance, string strPropertyName, object newValue)
    {  
        Type type = instance.GetType();
        FieldInfo fieldInfo = type.GetField(strPropertyName);

        fieldInfo.SetValue(instance, newValue);
    }

    /// <summary>
    /// Helper function which allows to retrieve the value of a field in a object at run time
    /// </summary>
    /// <param name="instance">Instance of the object you wish to retrieve the field value of</param>
    /// <param name="strPropertyName">Name of the field you wish to query</param>
    /// <returns>The value that was present in that field property</returns>
    public static object GetFieldValue(object instance, string strPropertyName)
    {
        Type type = instance.GetType();
        FieldInfo fieldInfo = type.GetField(strPropertyName);

        Debug.Log(instance);
        Debug.Log(strPropertyName);
        return fieldInfo.GetValue(instance);
    }

    /// <summary>
    /// Helper function which allows to retrieve the value of a field in the Constants object at run time
    /// </summary>
    /// <param name="strPropertyName">Name of the Constants field you wish to query</param>
    /// <returns></returns>
    public static object GetConstantField(string strPropertyName)
    {
        Type type = typeof(Constants);
        FieldInfo fieldInfo = type.GetField(strPropertyName);

        return fieldInfo.GetValue(null);
    }

    /// <summary>
    /// Helper function which takes a string and changes the first character to a lowercase char
    /// </summary>
    /// <param name="s">The string you wish to modify</param>
    /// <returns>Modified string with a lowercase first character</returns>
    static string LowercaseFirst(string s)
    {
        return char.ToLower(s[0]) + s.Substring(1);
    }
}
