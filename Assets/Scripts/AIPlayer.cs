using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    private Coroutine chasing;
    private Character target;

    IEnumerator DoEndTurnAfterDelay(float time, Character actor)
    {
        yield return new WaitForSeconds(time);
        EndTurn(actor);
    }

    IEnumerator DoChaseTarget(Character target)
    {
        // TODO: calculate path to each adjacent cell, follow the quickest

        HexCell closestAdjacent = target.Cell.GetNeighbours().Where(c => c != null).OrderBy(c => Vector3.Distance(current.Cell.Position, c.Position)).First();
        while (current.Cell != closestAdjacent)
        {
            // Path from ai to target
            HexPath path = PathToClosestAdjacent(target);
            if (path == null)
            {
                EndTurn(current);

                // Wait until next turn
                yield return new WaitUntil(() => GameManager.CurrentCharacter == current);
            }

            else
            {
                // The section of the path that is within the movement range of the ai
                HexPath inRangePath = path.Truncate(current.Stats.TimeUnits.Current);

                // Follow the path, wait until ai has stopped moving, then end turn
                current.FollowPath(inRangePath);
                yield return new WaitUntil(() => !current.IsMoving);
                EndTurn(current);

                // Wait until next turn
                yield return new WaitUntil(() => GameManager.CurrentCharacter == current);
            }
            
            // Recalculate the closest cell
            closestAdjacent = target.Cell.GetNeighbours().Where(c => c != null).OrderBy(c => Vector3.Distance(current.Cell.Position, c.Position)).First();
        }

        chasing = null;
        EndTurn(current);
    }

    /// <summary>
    /// Find the shortest path to a cell adjacent to the given target
    /// </summary>
    private HexPath PathToClosestAdjacent(Character target)
    {
        HexPath shortestPath = null;

        // Get each cell adjacent to the target
        foreach (HexCell adjacent in target.Cell.GetNeighbours())
        {
            // Get a path to the adjacent cell
            HexPath path = Pathfind.QuickestPath(current.Cell, adjacent, -1, current.Stats.Traverser);

            // If there is no shortest path yet recorded, make this one the shortest path
            if (shortestPath == null)
                shortestPath = path;    

            // Otherwise, compare this path to the shortest path to see if it is shorter
            else if (path != null && 
                     shortestPath != null && 
                     path.Cost <= shortestPath.Cost)
                shortestPath = path;
        }

        return shortestPath;
    }

    public override void Activate(Character actor)
    {
        base.Activate(actor);

        // If AI doesn't have a target, get one
        if (target == null)
            target = GameManager.Characters.Where(c => c != actor).Single();

        if (chasing == null)
            chasing = StartCoroutine(DoChaseTarget(target));
    }

    public override void EndTurn(Character actor)
    {
        GameManager.EndTurn(actor);
    }
}
