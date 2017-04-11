//using System;
//using System.Collections.Generic;
//using System.Linq;

//public class ChaseBehaviour : AIBehaviour
//{
//    private Character target;
//    private Attack attack;

//    public ChaseBehaviour(AIPlayer ai) 
//        : base(ai)
//    {
//        target = GameManager.Characters.Where(c => !Characters.Contains(c)).Single();
//    }

//    public override void Activate()
//    {
//        base.Activate();

//        // If not in range of target...
//        attack = Current.Attacks[0];
//        if (!attack.InRange(target.Cell))
//            MoveOrEndTurn();                // Move to attack range, or end turn if can't move

//        // Already in range...
//        else
//            AttackOrEndTurn();              // Attack, or end turn if unable to
//    }

//    public override void EndTurn()
//    {
//        // Stop listening for when character has finished moving
//        Current.FinishedMovement -= Current_FinishedMovement;

//        base.EndTurn();
//    }

//    private void MoveOrEndTurn()
//    {
//        // Move to attack range...
//        bool moving = Current.MoveToAttackRange(target, attack);

//        // If character is able to move, wait for them to finish moving
//        if (moving)
//            Current.FinishedMovement += Current_FinishedMovement;

//        // Otherwise, end turn.
//        else
//            EndTurn();
//    }

//    private void AttackOrEndTurn()
//    {
//        // Attack target
//        bool attacking = Current.Attack(target, attack);

//        // If able to attack target, wait for attack to finish
//        if (attacking)
//            Current.FinishedAttack += Current_FinishedAttack;

//        // Otherwise, end turn
//        else
//            EndTurn();
//    }

//    private void Current_FinishedMovement(object sender, CharacterMovementEventArgs e)
//    {
//        Current.FinishedMovement -= Current_FinishedMovement;
//        AttackOrEndTurn();
//    }

//    private void Current_FinishedAttack(object sender, EventArgs e)
//    {
//        Current.FinishedAttack -= Current_FinishedAttack;
//        EndTurn();
//    }
//}
