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
    public Transform hiHatNote;
    public Transform crashNote;
    public Transform snareDrumNote;
    public Transform hiTomNote;
    public Transform midTomNote;
    public Transform floorTomNote;
    public Transform rideNote;
    private Text sliderText;
    private Dictionary<int, float[][]> levels;
    bool inMenu;

    void Start()
    {
        LevelSetUp();
        StartGame(2);

        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddButton("Level 1", delegate() { StartGame(1); });
        DebugUIBuilder.instance.AddDivider();

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
    }

    private void LevelSetUp()
    {
        levels = new Dictionary<int, float[][]>();

        float[][] level1 = new float[][] { new float[] { 1, 2, 3, 4, 5, 5.5f, 6, 6.5f, 7, 7.5f, 8, 8.5f,
                9, 10, 11, 12, 13, 13.5f, 14, 14.5f, 15, 15.5f, 16, 16.5f,
                17, 18, 19, 20, 21, 21.5f, 22, 22.5f, 23, 23.5f, 24, 24.5f } };

        float[][] level2 = new float[][] { new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6, 6.5f },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { },
            new float[] { 2, 4, 6 } };

        levels.Add(1, level1);
        levels.Add(2, level2);
    }

    void StartGame(int level)
    {
        float[][] notePositions = levels[level]; 

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
            }
        }
       
        gameAssets.SetActive(true);

        inMenu = false;
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
}
