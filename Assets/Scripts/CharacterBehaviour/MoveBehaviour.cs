﻿using System;
using UnityEngine;
using HexMapPathfinding;

public class MoveBehaviour : CharacterBehaviour
{
    private HexPath path;
    private float distance;
    private float eta;
    private float time;
    private float t;

    public MoveBehaviour(Character character, HexPath path)
        : base(character)
    {
        this.path = path;
    }

    public override void Init()
    {
        base.Init();

        if (path.Count <= 1)
            SetState(new IdleBehaviour(character));

        else
        {
            distance = iTween.PathLength(path.Points);
            eta = distance / character.Stats.Speed;
            time = 0;
            t = 0;
        }
    }

    public override void Update()
    {
        base.Update();

        time += Time.deltaTime;
        t = time / eta;

        // Face along path, move along path
        character.transform.LookAt(iTween.PointOnPath(path.Points, t));
        iTween.PutOnPath(character.gameObject, path.Points, t);

        // Update the character's animation
        UpdateAnimator();

        // If reached destination
        if (t >= 1.0f)
        {
            // Pay for movement
            character.Stats.CurrentTimeUnits -= path.Cost;

            // Update reference to the currently occupied cell
            UpdateOccupiedCellFinal();

            SetState(new IdleBehaviour(character));
        }
    }

    private void UpdateOccupiedCellFinal()
    {
        // Has the character entered a new cell?
        HexCell newCell = HexGrid.GetCell(Transform.position);

        // Can't occupy a cell thats already occupied
        if (newCell.Occupant != null)
            throw new NotImplementedException("Cannot currently occupy cells that are already occupied");

        // Update ref to which cell is occupied
        Cell.Occupant = null;
        Cell = newCell;
        Cell.Occupant = character;
    }


    //private void UpdateOccupiedCell()
    //{
    //    // Has the character entered a new cell?
    //    HexCell newCell = HexGrid.GetCell(Transform.position);
    //    if (newCell != Cell)
    //    {
    //        // Calculate direction moved and cost to do so, subtract from time units
    //        HexDirection directionMoved = Cell.GetDirection(newCell);
    //        float moveCost = character.TraverseCost(directionMoved);
    //        character.Stats.CurrentTimeUnits -= moveCost;

    //        // Update ref to which cell is occupied
    //        Cell.Occupant = null;
    //        Cell = newCell;

    //        // Can't move through an occupied cell
    //        if (Cell.Occupant != null && Cell.Occupant != character)
    //            throw new NotImplementedException("Cannot currently traverse cells that are already occupied");

    //        Cell.Occupant = character;
    //    }
    //}

    private void UpdateAnimator()
    {
        if (Animator != null)
        {
            float t = time / eta;
            if (t >= 1f)
            {
                Animator.SetFloat("Speed", 0f);
                Animator.SetFloat("Direction", 0f);
            }

            else
            {
                // Speed
                Animator.SetFloat("Speed", 1f);

                // Focus point for model looking is 4 updates ahead of current position on path
                Vector3 focalPoint = iTween.PointOnPath(path.Points, (time + 4 * Time.smoothDeltaTime) / eta);

                // Calculate direction then angle of focal point
                Vector3 lookDir = (focalPoint - Transform.position).normalized;
                float lookAngle = Vector3.Angle(Transform.forward, lookDir);

                // If the angle to the left or right?
                float leftOrRight = MathExtension.AngleDir(Transform.forward, lookDir.normalized, Transform.up);

                Animator.SetFloat("Direction", (lookAngle / 20f) * leftOrRight);
            }
        }
    }
}
