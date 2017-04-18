using System;
using System.Linq;
using UnityEngine;

namespace HexMapPathfinding
{
    public static class HexCellExtension
    {
        /// <summary>
        /// Returns the positions of each corner of this hex in world space
        /// </summary>
        public static Vector3[] GetCorners(this HexCell cell)
        {
            Vector3[] corners = new Vector3[6];
            for (int i = 0; i < corners.Length; i++)
                corners[i] = cell.Position + HexMetrics.corners[i];

            return corners;
        }

        /// <summary>
        /// Gets the 6 cells adjacent to this cell. Null is returned where there is no cell in a direction
        /// </summary>
        public static HexCell[] GetNeighbours(this HexCell cell)
        {
            // Six neighbours, or null where there is no neighbour
            HexCell[] neighbours = new HexCell[6];

            // Get cell in each direction
            Array directionValues = Enum.GetValues(typeof(HexDirection));
            for (int i = 0; i < directionValues.Length; i++)
            {
                // Get each direction, get neighbour in that direction
                HexDirection direction = (HexDirection)directionValues.GetValue(i);
                neighbours[i] = cell.GetNeighbor(direction);
            }

            return neighbours;
        }

        /// <summary>
        /// Checks if the given cell is a neighbour of this cell
        /// </summary>
        public static bool IsNeighbour(this HexCell cell, HexCell other)
        {
            return cell.GetNeighbours().Contains(other);
        }

        /// <summary>
        /// Checks if there is a wall separating this cell from the cell in the given direction
        /// </summary>
        public static bool BorderWall(this HexCell cell, HexDirection direction)
        {
            // Neighbouring cell
            HexCell other = cell.GetNeighbor(direction);

            return (cell.Walled && !other.Walled) ||    // If this cell is walled and the neighbour isn't
                    (!cell.Walled && other.Walled);     // If the other cell is walled and this one isn't
        }

        /// <summary>
        /// Gets the direction that the other cell lies in from this cell.
        /// </summary>
        public static HexDirection GetDirection(this HexCell cell, HexCell other)
        {
            return cell.GetDirection(other.Position);
        }

        /// <summary>
        /// Gets the direction that the point lies in from this cell
        /// </summary>
        public static HexDirection GetDirection(this HexCell cell, Vector3 point)
        {
            HexDirection[] directions = (HexDirection[])Enum.GetValues(typeof(HexDirection));

            // Ignore cell height so that angle is based on x and z only
            Vector3 cellNoHeightPosition = cell.Position;
            cellNoHeightPosition.y = 0;
            point.y = 0;

            // Angle from forward vector  converted to an index
            float angle = Vector3.Angle(Vector3.forward, point - cellNoHeightPosition);
            int index = (int)(angle / 60);

            HexDirection direction = directions[index];
            return direction;
        }
    }
}