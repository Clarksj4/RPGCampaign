using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathfinding;
using UnityEngine;

namespace TileMap
{
    public interface ITile<T> : IGraphNode
    {
        Vector3 Position { get; }
        T Contents { get; set; }
    }


}
