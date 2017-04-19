using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadMap : MonoBehaviour
{
    public SaveLoadMenu Loader;
    public string filename = "Test";

    // Use this for initialization
    void Start ()
    {
        Loader.Load(Application.dataPath + "/" + filename + ".map");
	}
}
