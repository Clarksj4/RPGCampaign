using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChaseBehaviour : AIBehaviour
{
    private Character target;
    private HexPath path;

    public ChaseBehaviour(AIPlayer ai, Character target) 
        : base(ai)
    {
        this.target = target;
    }

    public override void Activate()
    {
        base.Activate();

        // Get path to target
        // tell character to move

        path = PathToWithinRange(target);
        //// Calculate path quickest to reach cell that is adjacent to target
        //HexPath path;
        //do
        //{
        //    path = PathToWithinRange(target);
        //    if (path == null)
        //    {
        //        EndTurn(current);

        //        // Wait until next turn
        //        yield return new WaitUntil(() => GameManager.CurrentCharacter == current);
        //    }

        //    else
        //    {
        //        // The section of the path that is within the movement range of the ai
        //        HexPath inRangePath = path.To(current.Stats.TimeUnits.Current);

        //        // Follow the path, wait until ai has stopped moving, then end turn
        //        current.Move(inRangePath);
        //        yield return new WaitUntil(() => !current.IsMoving);
        //        EndTurn(current);

        //        // Wait until next turn
        //        yield return new WaitUntil(() => GameManager.CurrentCharacter == current);
        //    }
        //} while (path == null || path.Destination != current.Cell);

        //// End turn if no path
        ////if (path.Destination == current.Cell)
        //EndTurn(current);
    }

    private HexPath PathToWithinRange(Character target)
    {
        List<Step> cellsInRange = Pathfind.CellsInRange(target.Cell, Current.Attack.range, Traverser.RangedAttack());
        List<HexCell> cells = cellsInRange.Select(s => s.Cell).ToList();

        if (cells.Contains(Current.Cell))
            return null;

        HexPath path = Pathfind.PathToAny(Current.Cell, cells, Current.Stats.Traverser);
        return path;
    }
}
