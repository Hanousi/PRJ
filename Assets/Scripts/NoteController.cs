using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * (-Time.deltaTime * 3));

        switch (gameObject.tag)
        {
            case "TomNote":
                transform.Translate(Vector3.down * Time.deltaTime / 3.83f);
                break;
            case "CymbalNote":
                transform.Translate(Vector3.right * Time.deltaTime / 6.3f);
                break;
            case "RideNote":
                transform.Translate(Vector3.left * Time.deltaTime / 6.3f);
                break;
        }
    }
}
