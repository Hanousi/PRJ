using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumController : MonoBehaviour {

    public GameObject note;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        note = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        note = null;    
    }
}
