using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to a invisible object behind the drumkit which catches notes which havent been hit successfully.
/// </summary>
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

    /// <summary>
    /// Called when the object collider that this script is attached to collides with another collider.
    /// Increments the correct drum counter in the dictionary and destorys the object to keep as little
    /// objects in the scene as possible.
    /// </summary>
    /// <param name="other">The collider instance that belongs to the other object</param>
    private void OnTriggerEnter(Collider other)
    {
        string otherTag = other.gameObject.tag;

        if(Array.IndexOf(Constants.noteCatcherInteractables, otherTag) > -1)
        {
            missedNotes[otherTag] = missedNotes[otherTag] + 1;
        }

        if (otherTag != "Drumstick" && otherTag != "Player")
        {
            Destroy(other.gameObject);
        }
    }

    public void resetResults()
    {
        List<string> keys = new List<string>(missedNotes.Keys);

        foreach(string key in keys)
        {
            missedNotes[key] = 0;
        }
    }
}
