using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileMap;
using UnityEngine;

public partial class HexGrid : ITileMap<Character>
{
    public ITile<Character> GetTile(Vector3 position)
    {
        return GetCell(position);
    }
}

