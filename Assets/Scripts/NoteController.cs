using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * (-Time.deltaTime * 2));

        switch (gameObject.tag)
        {
            case "TomNote":
                transform.Translate(Vector3.down * Time.deltaTime / 5.75f);
                break;
            case "CymbalNote":
                transform.Translate(Vector3.right * Time.deltaTime / 9.5f);
                break;
            case "RideNote":
                transform.Translate(Vector3.left * Time.deltaTime / 9.5f);
                break;
        }

        //if(gameObject.tag == "TomNote") {
        //    transform.Translate(Vector3.down * Time.deltaTime / 5.75f);
        //} else if(gameObject.tag == "CymbalNote") {
        //    transform.Translate(Vector3.right * Time.deltaTime / 9.5f);
        //}
    }
}
