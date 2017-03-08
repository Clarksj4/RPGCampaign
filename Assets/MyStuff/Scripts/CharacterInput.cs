using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterInput : MonoBehaviour
{
    [Tooltip("The character being controlled by this input")]
    public Character Character;
    [Tooltip("The hex grid the character exists on")]
    public HexGrid HexGrid;
    [Tooltip("The layer the hex grid is on. Used for raycasting")]
    public string HexGridLayer = "HexGrid";

    private HexCell target;
    private List<Step> movementRange;
    private List<Step> movementPath;
    private bool detectInput = true;

    private void Start()
    {
        foreach (Transform child in HexGrid.GetComponentsInChildren<Transform>())
            child.gameObject.layer = LayerMask.NameToLayer(HexGridLayer);
    }

    private void Update()
    {
        // If mouse over character cell - show movement range
        // If mouse over another cell - show path to cell
        // If mouse clicked on cell - move to cell

        if (detectInput)
        {
            // The cell the mouse pointer is over during this frame
            target = GetMousedCell();

            if (Character.Cell != null)
            {
                // If the left mouse button is clicked
                if (Input.GetMouseButtonDown(0) &&
                    target != Character.Cell &&
                    movementPath != null)
                {
                    Character.FollowPath(movementPath);     // Follow path

                    // Get rid of path and range because they are not valid anymore
                    movementPath = null;
                    movementRange = null;

                    // Pause responding to user input until character has walked the path
                    detectInput = false;
                    Character.DestinationReached += Character_DestinationReached;
                }

                else
                {
                    // If there is a cell under the cursor
                    if (target != null)
                    {
                        // If the targeted cell is the character's cell...
                        if (target == Character.Cell)
                        {
                            // Get the character's movement range
                            movementRange = Pathfind.CellsInRange(target, Character.TimeUnits.Current, Character.Traverser);
                            movementPath = null;    // Get rid of path so it is not drawn
                        }


                        // If the targeted cell is not the character's cell...
                        else
                        {
                            // Get the character's movement path to the targeted cell
                            movementPath = Pathfind.QuickestPath(Character.Cell, target, Character.TimeUnits.Current, Character.Traverser);
                            movementRange = null;   // Get rid of range so it is not drawn
                        }
                    }
                }
            }
        }
    }

    private void Character_DestinationReached(object sender, System.EventArgs e)
    {
        // Allow user input now that character has reached desination
        detectInput = true;
        Character.DestinationReached -= Character_DestinationReached;
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
                Color colour = Color.Lerp(Color.green, Color.red, step.CostTo / Character.TimeUnits.Current);
                DrawCell(step.Cell, colour);
            }

            Gizmos.color = Color.red;
            iTween.DrawPath(movementPath.Select(s => s.Cell.Position + Vector3.up).ToArray());
        }

        // Draw all cells in range
        if (movementRange != null)
        {
            foreach (Step step in movementRange)
            {
                // 'Green-er' closer to start, 'red-er' towards end
                Color colour = Color.Lerp(Color.green, Color.red, step.CostTo / Character.TimeUnits.Current);

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

            // If there's a hit it must be the hexmesh (cause nothing else in scene)
            if (Physics.Raycast(ray, out hitInfo, layer))
                return HexGrid.GetCell(hitInfo.point);
        }

        // No cell clicked
        return null;
    }
}
