using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

    private Transform target;
    private float speed;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
