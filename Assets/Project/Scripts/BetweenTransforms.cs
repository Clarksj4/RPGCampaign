using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BetweenTransforms : MonoBehaviour
{
    public Transform[] Transforms;
	
	// Update is called once per frame
	void Update ()
    {
        if (Transforms.Length > 0)
        {
            Vector3 newPosition = Vector3.zero;
            foreach (Vector3 position in Transforms.Select(t => t.position))
                newPosition += position;

            transform.position = newPosition / Transforms.Length;
        }
	}
}
