using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DrumstickController : MonoBehaviour {

    // Use this for initialization
    void Start () {
         
    }
	
	// Update is called once per frame
	void Update () {
        		   
	}

    void OnTriggerEnter(Collider other) {
        AudioSource audioData = other.GetComponent<AudioSource>();
        DrumController drumController = other.GetComponent<DrumController>();
        GameObject note = drumController.note;
        if(note)
        {
            Destroy(note);
        }
        audioData.Play(0);
    }
}
