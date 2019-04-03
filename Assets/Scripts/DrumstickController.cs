using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script component attached to the beaters of the instrument. 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DrumstickController : MonoBehaviour {

    public delegate void NoteMiss(string noteName);
    public static event NoteMiss OnMiss;
    private char inHand;

    // Use this for initialization
    void Start () {
         
    }
	
	// Update is called once per frame
	void Update () {
        		   
	}

    /// <summary>
    /// Function called when the object's collider that this script is attached to collides with another.
    /// During collision, there is check for a note, if one exists it is destroyed otherwise a ghost hit has
    /// been executed.
    /// </summary>
    /// <param name="other">The collider instance that belongs to the other object</param>
    void OnTriggerEnter(Collider other) {
        if (Array.IndexOf(Constants.drumstickInteractables, other.tag) > -1) {
            OVRInput.SetControllerVibration(1, 1, inHand == 'R' ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch);
            AudioSource audioData = other.GetComponent<AudioSource>();
            audioData.Play(0);

            try
            {
                DrumController drumController = other.GetComponent<DrumController>();
                GameObject note = drumController.note;
                if (note)
                {
                    Destroy(note);
                }
                else
                {
                    OnMiss(other.tag);
                }
            } catch(NullReferenceException e)
            {

            }
         
        } else if (other.tag == "Controller")
        {
            inHand = other.name[0];
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OVRInput.SetControllerVibration(0, 0, inHand == 'R' ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch);
    }
}
