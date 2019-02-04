using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(Time.deltaTime);
        transform.Translate(Vector3.forward * -Time.deltaTime);

        if(gameObject.tag == "TomNote")
        {
            transform.Translate(Vector3.down * Time.deltaTime/11.5f);
        };
    }
}
