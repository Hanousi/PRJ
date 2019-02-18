using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            OnMiss(note.tag);
        }
    }
}
