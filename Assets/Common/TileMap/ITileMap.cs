using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TileMap
{
    public interface ITileMap<T>
    {
        ITile<T> GetTile(Vector3 position);
    }
}
