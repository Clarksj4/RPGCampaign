using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChaseBehaviour : AIBehaviour
{
    private Character target;
    private HexPath path;

    public ChaseBehaviour(AIPlayer ai) 
        : base(ai)
    {
        target = GameManager.Characters.Where(c => !Characters.Contains(c)).Single();
    }

    public override void Activate()
    {
        base.Activate();

        // If not in range of target...
        Attack attack = Current.Attacks[0];
        if (!attack.InRange(target.Cell))
        {
            // Move to attack range...
            bool moving = Current.MoveToAttackRange(target, attack);

            // If character is able to move, wait for them to finish moving
            if (moving)
                Current.FinishedMovement += Current_FinishedMovement;
            
            // Otherwise, end turn.
            else
                EndTurn();
        }

        // Already in range...
        else
        {
            // Do attack!
            // Wait for attack to complete
            // End turn
            EndTurn();
        }
    }

    public override void EndTurn()
    {
        // Stop listening for when character has finished moving
        Current.FinishedMovement -= Current_FinishedMovement;

        base.EndTurn();
    }

    private void Current_FinishedMovement(object sender, CharacterMovementEventArgs e)
    {
        Attack attack = Current.Attacks[0];
        if (Current.Stats.CurrentTimeUnits >= attack.Cost)
        {
            
        }

        // End turn when character has finished moving
        EndTurn();
    }
}
