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

        HexGrid grid = FindObjectOfType<HexGrid>();
        foreach (Character character in FindObjectsOfType<Character>())
        {
            HexCell cell = grid.GetCell(character.transform.position);
            character.Tile = cell;
            cell.Contents = character;

            character.transform.position = cell.Position;
            character.transform.LookAt(character.Facing);
        }
    }
}
