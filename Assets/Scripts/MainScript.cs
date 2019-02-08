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
    }

    private void LevelSetUp()
    {
        levels = new Dictionary<int, float[][]>();

        float[][] level1 = new float[][] { new float[] { 1, 2, 3, 4, 5, 5.5f, 6, 6.5f, 7, 7.5f, 8, 8.5f,
                9, 10, 11, 12, 13, 13.5f, 14, 14.5f, 15, 15.5f, 16, 16.5f,
                17, 18, 19, 20, 21, 21.5f, 22, 22.5f, 23, 23.5f, 24, 24.5f } };

        float[][] level2 = new float[][] { new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6, 6.5f }, new float[] { }, new float[] { 2, 4, 6 } };

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

            switch(i)
            {
                case 0:
                    //TODO: change these forloops into a single function
                    foreach (float position in drumNotePositions)
                    {
                        Instantiate(hiHatNote, new Vector3((float)(0.59 + (position * 0.106)) * -1, 0.8f, (float)position * 2), Quaternion.identity);
                    }

                    break;
                case 1:
                    foreach (float position in drumNotePositions)
                    {
                        Instantiate(crashNote, new Vector3((float)(0.18 + (position * 0.106)) * -1, 1.12f, (float) ((position * 2) + 0.92)), Quaternion.identity);
                    }

                    break;
                case 2:
                    foreach (float position in drumNotePositions)
                    {
                        Instantiate(snareDrumNote, new Vector3(-0.07f, 0.6f, (float)((position * 2) + 0.28)), Quaternion.identity);
                    }

                    break;
            }

            
        }
       
        //Instantiate(hiHatNote, new Vector3(-0.74f, 0.8f, 3), Quaternion.identity);
        gameAssets.SetActive(true);

        inMenu = false;
    }
}
