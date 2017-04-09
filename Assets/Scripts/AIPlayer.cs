using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;

public class AIPlayer : Player
{
    private IBehaviourStrategy behaviour;

    protected override void Awake()
    {
        base.Awake();

        // [PLACEHOLDER] TODO: Behaviour factory assembles behaviour trees and instantiates behaviour classes
        SetBehaviour(new AggressiveBehaviour());
    }

    /// <summary>
    /// Sets this AI's behaviour strategy
    /// </summary>
    public void SetBehaviour(IBehaviourStrategy behaviour)
    {
        this.behaviour = behaviour;
    }

    public override void Activate(Character actor)
    {
        base.Activate(actor);

        // Behaviour handler is notified that it's its turn
        behaviour.Activate(actor);

        // Traverse tree until tree fails or succeeds
        StartCoroutine(ProcessTurn());
    }

    IEnumerator ProcessTurn()
    {
        // Behaviour strategy traverses behaviour tree and reports its status
        BehaviourTreeStatus status = behaviour.Update();

        // Continuously update the behaviour tree until it reports that it is done
        while (status == BehaviourTreeStatus.Running)
        {
            yield return null;

            status = behaviour.Update();
        }

        // Same as above logic, but condensed to one line; therefore, harder to debug
        //yield return new WaitUntil(() => behaviour.Update() != BehaviourTreeStatus.Running);

        // End turn once the behaviour tree has completed its actions 
        EndTurn();
    }
}
