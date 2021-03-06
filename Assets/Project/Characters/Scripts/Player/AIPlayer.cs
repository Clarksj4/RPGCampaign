﻿using System.Collections;
using UnityEngine;
using FluentBehaviourTree;

public class AIPlayer : Player
{
    public Player TargetPlayer;

    private IBehaviourStrategy behaviour;

    protected override void Awake()
    {
        base.Awake();

        SetBehaviour(new AggressiveBehaviour());
    }

    /// <summary>
    /// Sets this AI's behaviour strategy
    /// </summary>
    public void SetBehaviour(IBehaviourStrategy behaviour)
    {
        this.behaviour = behaviour;
    }

    public override void PawnStart(Character pawn)
    {
        base.PawnStart(pawn);

        // Behaviour handler is notified that it's its turn
        behaviour.PawnStart(pawn);

        // Traverse tree until tree fails or succeeds
        StartCoroutine(ProcessTurn());
    }

    public override void PawnDie(Character pawn)
    {
        // Fade out
        pawn.Model.Fade(() => CleanUpPawn(pawn));
    }

    private void CleanUpPawn(Character pawn)
    {
        turnSystem.Remove(pawn.GetComponent<TurnBasedEntity>());
        Destroy(pawn.gameObject);
    }

    IEnumerator ProcessTurn()
    {
        // Think for a second
        yield return new WaitForSeconds(1);

        // Behaviour strategy traverses behaviour tree and reports its status
        BehaviourTreeStatus status = behaviour.Update();

        // Continuously update the behaviour tree until it reports that it is done
        while (status != BehaviourTreeStatus.Success)
        {
            yield return null;

            status = behaviour.Update();
        }

        // End turn once the behaviour tree has completed its actions 
        turnSystem.EndTurn();
    }
}
