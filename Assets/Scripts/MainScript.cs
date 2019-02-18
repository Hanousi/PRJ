using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
    private Text sliderText;
    private GameObject currentUI;
    private bool inGame = false;
    private float timeLeft = -1;
    private float[][] currentLevel;
    private Dictionary<int, float[][]> levels;
    private Dictionary<string, int> ghostHits = new Dictionary<string, int>() {
        { "HiHatNote", 0 },
        { "CrashNote", 0 },
        { "SnareDrumNote", 0 },
        { "HiTomNote", 0 },
        { "MidTomNote", 0},
        { "FloorTomNote", 0},
        { "RideNote", 0 }
    };
    bool inMenu;

    void Start()
    {
        LevelSetUp();
        StartGame(3);
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
            
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
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

                NoteCatcherController noteCatcherController = noteCatcher.GetComponent<NoteCatcherController>();
                Dictionary<string, string> results = getGameResults(noteCatcherController.missedNotes);

                Destroy(currentUI);
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
                inMenu = true;
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
        levels = new Dictionary<int, float[][]>();

        float[][] level1 = new float[][] { new float[] { 1, 2, 3, 4, 5, 5.5f, 6, 6.5f, 7, 7.5f, 8, 8.5f,
                9, 10, 11, 12, 13, 13.5f, 14, 14.5f, 15, 15.5f, 16, 16.5f,
                17, 18, 19, 20, 21, 21.5f, 22, 22.5f, 23, 23.5f, 24, 24.5f } };

        float[][] level2 = new float[][] { new float[] { 1, 2, 3, 4, 5, 5.5f, 6, 6.5f, 7, 7.5f, 8, 8.5f },
            new float[] { 9, 10, 11, 12, 13, 13.5f, 14, 14.5f, 15, 15.5f, 16, 16.5f },
            new float[] { 17, 18, 19, 20 },
            new float[] { 21, 21.5f, 22, 22.5f, 23, 23.5f, 24, 24.5f },
            new float[] { 25, 26, 29, 29.5f, 30, 30.5f },
            new float[] { 27, 28, 31, 31.5f, 32, 32.5f },
            new float[] {  },
            new float[] { 20 }
        };

        float[][] level3 = new float[][] { new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6, 6.5f },
            new float[] { },
            new float[] { 2, 4, 6 },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { 6 }
        };

        float[][] level4 = new float[][] { new float[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 25.5f, 26.5f },
            new float[] { },
            new float[] { 3, 7, 11, 15, 19, 23, 26 },
            new float[] { },
            new float[] { },
            new float[] { 1, 5, 9, 13, 17, 21, 25 },
            new float[] { },
            new float[] { 20 }
        };

        float[][] level5 = new float[][] { new float[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 25.5f, 26.5f },
            new float[] { },
            new float[] { 3, 7, 11, 15, 19, 23, 26 },
            new float[] { },
            new float[] { },
            new float[] { 1, 5, 9, 13, 17, 21, 25 },
            new float[] { },
            new float[] { 6 }
        };

        levels.Add(1, level1);
        levels.Add(2, level2);
        levels.Add(3, level3);
        levels.Add(4, level4);
        levels.Add(5, level5);
    }

    void StartGame(int level)
    {
        float[][] notePositions = levels[level];
        currentLevel = notePositions;

        //DebugUIBuilder.instance.Hide();
        for(int i = 0; i < notePositions.Length; i++)
        {
            float[] drumNotePositions = notePositions[i];

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
                default:
                    timeLeft = drumNotePositions[0];

                    break;
            }
        }
       
        gameAssets.SetActive(true);
        inGame = true;

        inMenu = false;
    }

    private void buildMainMenu()
    {
        currentUI = Instantiate(canvasWithDebug, new Vector3(0.32f, -0.46f, 2.43f), Quaternion.identity);

        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddButton("Level 1", delegate () { StartGame(1); });
        DebugUIBuilder.instance.AddDivider();
    }

    void createNote(Transform note, float[] drumNotePositions, float x, float xOffset, float xModifier, float y, float yOffset, float yModifier, float zOffset)
    {
        foreach (float position in drumNotePositions)
        {
            Instantiate(note, new Vector3(
                x == 0 ? (float)(xOffset + (position * xModifier)) : x,
                y == 0 ? (float)(yOffset + (position * yModifier)) : y,
                (float)((position * 2) + zOffset)
                ), Quaternion.identity);
        }
    }

    void getHitRate(string noteName, int totalNotes, Dictionary<string, int> missedNotes, Dictionary<string, string> results)
    {
        if (totalNotes == 0)
        {
            results.Add(noteName, "100%");
        }
        else
        {
            double hitRate = (totalNotes - missedNotes[noteName]) / totalNotes;
            results.Add(noteName, (hitRate * 100).ToString() + "%");
        }


    }
}
