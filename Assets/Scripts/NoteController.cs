using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to the dynamically instantiated notes used for the drum exercises.
/// </summary>
public class NoteController : MonoBehaviour {

    /// <summary>
    /// Target object which is placed behind the drumkit which the note should move towards. 
    /// </summary>
    private Transform target;
    /// <summary>
    /// The speed of which the note should move towards it's target at.
    /// </summary>
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
