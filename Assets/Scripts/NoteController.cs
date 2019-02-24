using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        switch (gameObject.tag)
        {
            case "HiTomNote":
                transform.Translate(Vector3.forward * (-Time.deltaTime * 3));
                break;
            case "MidTomNote":
                transform.Translate(Vector3.forward * (-Time.deltaTime * 3));
                transform.Translate(Vector3.down * Time.deltaTime / 3.83f);
                break;
            case "HiHatNote":
                transform.Translate(Vector3.forward * (-Time.deltaTime * 3));
                break;
            case "CrashNote":
                transform.Translate(Vector3.forward * (-Time.deltaTime * 3));
                transform.Translate(Vector3.right * Time.deltaTime / 6.3f);
                break;
            case "RideNote":
                transform.Translate(Vector3.forward * (-Time.deltaTime * 3));
                transform.Translate(Vector3.left * Time.deltaTime / 6.3f);
                break;
        }
    }
}
