using System;
using UnityEngine;
using Pathfinding;

public class MoveBehaviour : CharacterBehaviour
{
    private HexGrid grid;

    private Path path;
    private Vector3[] points;
    private float distance;
    private float eta;
    private float time;
    private float t;

    public MoveBehaviour(Character character, Path path)
        : base(character)
    {
        this.path = path;
        points = path.GetPoints();
        grid = GameObject.FindObjectOfType<HexGrid>();
    }

    public override void Init()
    {
        base.Init();

        if (path.Count <= 1)
            SetState(new IdleBehaviour(character));

        else
        {
            // Pay for movement
            character.Stats.SpendTimeUnits(path.Cost);

            distance = iTween.PathLength(points);
            time = 0;
            t = 0;

            if (path.Count > 4)
            {
                eta = distance / character.Model.RunSpeed;
                character.Model.Run(true);
            }
                
            else
            {
                eta = distance / character.Model.WalkSpeed;
                character.Model.Walk(true);
            }
        }
    }

    public override void Update()
    {
        base.Update();

        time += Time.deltaTime;
        t = time / eta;

        // Face along path, move along path
        character.transform.LookAt(iTween.PointOnPath(points, t));
        iTween.PutOnPath(character.gameObject, points, t);

        // If reached destination
        if (t >= 1.0f)
            DestinationReached();
    }

    private void DestinationReached()
    {
        Vector3 lookPosition = character.transform.position + character.transform.forward;
        lookPosition.y = character.transform.position.y;
        character.transform.LookAt(lookPosition);

        // Update reference to the currently occupied cell
        UpdateOccupiedCellFinal();

        // Stop walking
        character.Model.Walk(false);
        character.Model.Run(false);

        SetState(new IdleBehaviour(character));
    }

    private void UpdateOccupiedCellFinal()
    {
        // Has the character entered a new cell?

        HexCell newCell = grid.GetCell(character.transform.position);

        // Can't occupy a cell thats already occupied
        if (newCell.Contents != null)
            throw new NotImplementedException("Cannot currently occupy cells that are already occupied");

        // Update ref to which cell is occupied
        character.Tile.Contents = null;
        character.Tile = newCell;
        newCell.Contents = character;
    }
}
