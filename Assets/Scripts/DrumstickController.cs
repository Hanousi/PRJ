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
        AudioSource audioData = other.GetComponent<AudioSource>();
        audioData.Play(0);

        DrumController drumController = other.GetComponent<DrumController>();
        GameObject note = drumController.note;
        if(note)
        {
            Destroy(note);
        }
        else
        {
            OnMiss(other.tag);
        }
    }
}
