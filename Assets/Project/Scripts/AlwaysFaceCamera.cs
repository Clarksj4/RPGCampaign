using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    public Camera Camera;

    private void Awake()
    {
        if (Camera == null)
            Camera = Camera.main;
    }

	// Update is called once per frame
	void Update ()
    {
        transform.forward = Camera.transform.forward;
	}
}
