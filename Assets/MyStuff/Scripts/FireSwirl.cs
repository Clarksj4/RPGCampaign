using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSwirl : MonoBehaviour
{
    public float AngularVelocity = 360;

	// Update is called once per frame
	void Update ()
    {
        transform.RotateAround(transform.parent.position, transform.parent.forward, AngularVelocity * Time.deltaTime);
	}
}
