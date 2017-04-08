using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;

public class AIPlayer : Player
{
    private IBehaviourTreeNode behaviourTree;

    private Character target;
    private Attack attack;
    private HexPath path;
    private bool setAlongPath = false;
    private bool toldToAttack;

    private void Start()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        behaviourTree = builder
            .Sequence("Sequence")
                .Do("Pick spell", t => PickSpell())
                .Sequence("Sequence")
                    .Do("Find target", t => FindTarget())
                    .Sequence("Sequence")
                        .Selector("Range")
                            .Condition("In range?", t => InRange())
                            .Sequence("Get in range")
                                .Do("Find path to in range position", t => FindPath())
                                .Condition("Enough TU for move and attack?", t => WithinCost())
                                .Do("Move into range", t => FollowPath())
                            .End()
                        .End()
                        .Selector("Cast spell")
                            .Condition("Enough TU for attack?", t => current.Stats.CurrentTimeUnits >= attack.Cost)
                            .Do("Attack!", t => Attack())
                        .End()
                    .End()
                .End()
            .End()
            .Build();
    }

    public override void Activate(Character actor)
    {
        base.Activate(actor);

        StartCoroutine(ProcessTurn());
    }

    IEnumerator ProcessTurn()
    {
        BehaviourTreeStatus status = behaviourTree.Tick(new TimeData(Time.deltaTime));
        while (status == BehaviourTreeStatus.Running)
        {
            yield return null;

            status = behaviourTree.Tick(new TimeData(Time.deltaTime));
        }

        //yield return new WaitUntil(() => behaviourTree.Tick(new TimeData(Time.deltaTime)) != BehaviourTreeStatus.Running);
        EndTurn();
    }

    private BehaviourTreeStatus PickSpell()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        if (current.Attacks.Length > 0)
        {
            attack = current.Attacks[0];
            result = BehaviourTreeStatus.Success;
        }
            
        return result;
    }

    private BehaviourTreeStatus FindTarget()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        // Find a character that is not one of this player's characters
        List<Character> enemies = GameManager.Characters.Where(c => !characters.Contains(c)).ToList();
        target = enemies[0];

        // If failed to find an enemy
        if (target != null)
            result = BehaviourTreeStatus.Success;

        return result;
    }

    private bool InRange()
    {
        bool inRange = attack.InRange(target.Cell);
        return inRange;
    }

    private BehaviourTreeStatus FindPath()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        // Find a path from the current characters cell quickest to reach cell that is in range of the target for the given attack
        path = Pathfind.ToWithinRange(current.Cell, target.Cell, attack.Range, current.Stats.Traverser, attack.Traverser);

        // Is the path legit?
        if (path != null && path.Count >= 2)
            result = BehaviourTreeStatus.Success;

        return result;
    }

    private bool WithinCost()
    {
        bool withinCost = current.Stats.CurrentTimeUnits >= path.Cost + attack.Cost;
        return withinCost;
    }

    private BehaviourTreeStatus FollowPath()
    {
        // Result by default is RUNNING rather than failure
        BehaviourTreeStatus result = BehaviourTreeStatus.Running;

        // If character hasn't been told to move yet...
        if (!setAlongPath)
        {
            // Move character
            current.Move(path);
            setAlongPath = true;
        }

        // Has character finished moving?
        else if (!current.IsMoving)
        {
            setAlongPath = false;
            result = BehaviourTreeStatus.Success;
        }

        return result;
    }

    private BehaviourTreeStatus Attack()
    {
        // Result by default is RUNNING rather than failure
        BehaviourTreeStatus result = BehaviourTreeStatus.Running;

        // Has character finished attack?
        if (!current.IsAttacking && toldToAttack)
        {
            toldToAttack = false;
            result = BehaviourTreeStatus.Success;
        }

        // If the character has not been told to attack yet....
        else if (!toldToAttack)
        {
            current.Attack(target, attack);
            toldToAttack = true;
        }

        return result;
    }
}
