using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script place on the colliders of the drums to figure out whether or not there exists
/// a note within it.
/// </summary>
public class DrumController : MonoBehaviour {

    public GameObject note = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Drumstick" && other.gameObject.tag != "Player")
        {
            note = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Drumstick" && other.gameObject.tag != "Player")
        {
            note = null;
        }
    }
}
