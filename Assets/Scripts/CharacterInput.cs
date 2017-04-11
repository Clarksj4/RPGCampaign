using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterInput : MonoBehaviour
{
    [Tooltip("The character being controlled by this input")]
    public Character Selected;
    [Tooltip("The hex grid the character exists on")]
    public HexGrid HexGrid;
    [Tooltip("The layer the hex grid is on. Used for raycasting")]
    public string HexGridLayer = "HexGrid";

    private HexCell target;
    private ICollection<Step> movementRange;
    private HexPath movementPath;

    private void Start()
    {
        // Set all cell to hex grid layer
        foreach (Transform child in HexGrid.GetComponentsInChildren<Transform>())
            child.gameObject.layer = LayerMask.NameToLayer(HexGridLayer);
    }

    private void Update()
    {
        target = GetMousedCell();
        if (target != null)                         // IF the mouse is over a cell
        {
            Character occupant = target.Occupant;
            if (occupant != null &&                 // IF the targeted cell is occupied
                !occupant.IsMoving)                 // AND the occupant is not currently moving
            {
                // Get the character's movement range
                movementRange = Pathfind.Area(target, occupant.Stats.CurrentTimeUnits, occupant.Stats.Traverser);
                movementPath = null;    // Get rid of path so it is not drawn
            }

            else if (!Selected.Controller.IsTurn)   // IF its not the selected characters turn
            {
                // Get rid of range and path so they are not drawn
                movementRange = null;       
                movementPath = null;
            }
            
            else if (!Selected.IsMoving)            // IF its the selected character's turn AND the character is not moving    
            {
                if (Input.GetMouseButton(0))
                {
                    Selected.Move(movementPath);     // Follow path

                    // Get rid of path and range because they are not valid anymore
                    movementPath = null;
                    movementRange = null;
                }

                else
                {
                    // Get the character's movement path to the targeted cell
                    movementPath = Pathfind.To(Selected.Cell, target, Selected.Stats.CurrentTimeUnits, Selected.Stats.Traverser);
                    movementRange = null;   // Get rid of range so it is not drawn
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw destination
        if (target != null)
            DrawCell(target, Color.white);

        // Draw movementPath
        if (movementPath != null)
        {
            foreach (Step step in movementPath)
            {
                // Colour changes from green to red during the path
                Color colour = Color.Lerp(Color.green, Color.red, step.CostTo / Selected.Stats.CurrentTimeUnits);
                DrawCell(step.Cell, colour);
            }
        }

        // Draw all cells in range
        if (movementRange != null)
        {
            foreach (Step step in movementRange)
            {
                // 'Green-er' closer to start, 'red-er' towards end
                Color colour = Color.Lerp(Color.green, Color.red, step.CostTo / Selected.Stats.CurrentTimeUnits);

                DrawCell(step.Cell, colour);
            }
        }
    }

    /// <summary>
    /// Draw a cell in the given colour with Gizmo lines
    /// </summary>
    private void DrawCell(HexCell cell, Color colour)
    {
        // Set colour, remember old colour
        Color oldColour = Gizmos.color;
        Gizmos.color = colour;

        // Draw line from each vert to next vert
        Vector3[] corners = cell.GetCorners();
        for (int i = 0; i < corners.Length - 1; i++)
            Gizmos.DrawLine(corners[i] + Vector3.up, corners[i + 1] + Vector3.up);
        Gizmos.DrawLine(corners.Last() + Vector3.up, corners.First() + Vector3.up);

        // Reset colour
        Gizmos.color = oldColour;
    }

    /// <summary>
    /// Get the hex cell the mouse is currently over
    /// </summary>
    private HexCell GetMousedCell()
    {
        // If mouse not over UI
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // Raycast onto hexmesh (because it's the only thing in the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            int layer = 1 << LayerMask.NameToLayer(HexGridLayer);

            // If there's a hit it must be the hexmesh
            if (Physics.Raycast(ray, out hitInfo, layer))
                return HexGrid.GetCell(hitInfo.point);
        }

        // No cell clicked
        return null;
    }
}
