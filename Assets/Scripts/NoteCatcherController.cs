using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteCatcherController : MonoBehaviour {

    public Dictionary<string, int> missedNotes = new Dictionary<string, int>() {
        { "HiHatNote", 0 },
        { "CrashNote", 0 },
        { "SnareDrumNote", 0 },
        { "HiTomNote", 0 },
        { "MidTomNote", 0},
        { "FloorTomNote", 0},
        { "RideNote", 0 }
    };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        missedNotes[other.gameObject.tag] = missedNotes[other.gameObject.tag] + 1;

        if (other.gameObject.tag != "Drumstick" && other.gameObject.tag != "Player")
        {
            Destroy(other.gameObject);
        }
    }
}
