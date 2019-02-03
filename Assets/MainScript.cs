using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Show off all the Debug UI components.
public class MainScript : MonoBehaviour
{
    bool inMenu;
    private Text sliderText;

    void Start()
    {
        DebugUIBuilder.instance.AddDivider();
        DebugUIBuilder.instance.AddButton("Start Game", StartGame);
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

    void StartGame()
    {
        Debug.Log("Button pressed");
    }
}
