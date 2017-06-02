using System;
using System.Collections.Generic;
using System.Linq;
using FluentBehaviourTree;
using UnityEngine;
using Pathfinding;

public class AggressiveBehaviour : IBehaviourStrategy
{
    private IBehaviourTreeNode behaviourTree;

    private Character[] Characters { get { return GameObject.FindObjectsOfType<Character>(); } }
    private AIPlayer Player { get { return GameObject.FindObjectOfType<AIPlayer>(); } }

    private Character current;
    private Character target;
    private Ability ChosenAbility { get { return prioritizedAbilites[abilityIndex]; } }
    private List<Ability> prioritizedAbilites;
    private int abilityIndex = 0;
    private Path path;
    private bool moveOrderGiven = false;
    private bool abilityOrderGiven = false;

    public AggressiveBehaviour()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        behaviourTree = builder
            .Sequence("Sequence")
                .Condition("Is Idle?", t => IsIdle())
                .Selector("Choose action")
                    .Splice(ActTree())
                    .Splice(MoveTree())
                    .Splice(EndTurnTree())
                .End()
                .Splice(EndTurnTree())
            .End()
        .Build();
    }

    public void PawnStart(Character current)
    {
        this.current = current;
    }

    public BehaviourTreeStatus Update()
    {
        return behaviourTree.Tick(new TimeData(Time.deltaTime));
    }

    private IBehaviourTreeNode ActTree()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        IBehaviourTreeNode actTree = builder
            .Sequence("Act")
                .Do("Select closest enemy", t => FindTarget())
                .Do("Prioritize abilities", t => PrioritizeAbilities())
                .Repeater("Repeat")
                    .Selector("Try abilities")
                        .Repeater("Repeat")
                            .Sequence("Try ability")
                                .Selector("Range find")
                                    .Condition("In range?", t => InRange())
                                    .Sequence("Advance")
                                        .Do("Get path", t => FindPath())
                                        .Condition("Enough TU for move AND ability?", t => MoveAndAbilityWithinCost())
                                        .Do("Move into range", t => FollowPath())
                                    .End()
                                .End()
                                .Sequence("Ability")
                                    .Condition("Enough TU for ability?", t => AbilityWithinCost())
                                    .Do("Use ability", t => UseAbility())
                                .End()
                            .End()
                        .End()
                        .Do("Get next ability", t => GetNextAbility())
                    .End()
                .End()
            .End()
        .Build();

        return actTree;
    }

    private IBehaviourTreeNode MoveTree()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        IBehaviourTreeNode moveTree = builder
            .Sequence("Move")
                .Do("Select highest priority ability", t => SelectHighestPriorityAbility())
                .Do("Find path", t => FindPath())
                .Do("Move towards enemy", t => FollowPath())
            .End()
        .Build();

        return moveTree;
    }   

    private IBehaviourTreeNode EndTurnTree()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        IBehaviourTreeNode endTurnTree = builder
            .Sequence("End Turn")
                .Do("Reset parameters", t => ResetParameters())
            .End()
        .Build();

        return endTurnTree;
    }

    private bool IsIdle()
    {
        bool isIdle = current.IsIdle;
        return isIdle;
    }

    private BehaviourTreeStatus ResetParameters()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Success;

        // Reset behaviour tree variables
        target = null;
        path = null;
        abilityIndex = 0;
        moveOrderGiven = false;
        abilityOrderGiven = false;

        return result;
    }

    private BehaviourTreeStatus SelectHighestPriorityAbility()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Success;

        abilityIndex = 0;

        return result;
    }

    private BehaviourTreeStatus PrioritizeAbilities()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Success;

        // Order abilities by damage, then cost
        prioritizedAbilites = current.Abilities.OrderByDescending(a => a.Damage)
                                               .ThenBy(a => a.Cost).ToList();

        // Return success
        return result;
    }

    private BehaviourTreeStatus GetNextAbility()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        // Proceed to next ability in list
        abilityIndex++;

        if (abilityIndex < prioritizedAbilites.Count)
            result = BehaviourTreeStatus.Success;

        return result;
    }

    /// <summary>
    /// Finds an enemy target to attack. Picks the first non-allied character from the scene hierarchy
    /// </summary>
    /// <returns>Success if there is a non-allied character in the scene, failure otherwise</returns>
    private BehaviourTreeStatus FindTarget()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        // Find a character that is not one of this player's characters
        Character[] characters = GameObject.FindObjectsOfType<Character>();
        Character[] enemies = characters.Where(c => c.Controller != current.Controller).ToArray();

        // If there is an enemy
        if (enemies.Length > 0)
        {
            target = enemies[0];
            result = BehaviourTreeStatus.Success;
        }

        return result;
    }

    /// <summary>
    /// Checks if the chosen attack is in range of the chosen target
    /// </summary>
    /// <returns>True if the attack is currently in range</returns>
    private bool InRange()
    {
        bool inRange = ChosenAbility.InRange(current.Tile, target.Tile);
        return inRange;
    }

    /// <summary>
    /// Finds a path from the current character's position to the quickest cell that is in range of the target for the chosen 
    /// attack
    /// </summary>
    /// <returns>Success if there is a path to an in range cell, failure otherwise</returns>
    private BehaviourTreeStatus FindPath()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        if (target != null)
        {
            // Get area around target which would put AI character in range for attack
            ICollection<PathStep> area = Pathfind.Area(target.Tile, ChosenAbility.MaximumRange, ChosenAbility.Traverser);

            // Find a path from the current characters cell to the quickest to reach cell that is in range of the target for 
            // the given attack
            path = Pathfind.ToArea(current.Tile, area.Select(s => s.Node), current.Stats.Traverser);

            // Is the path legit?
            if (path != null && path.Count >= 2)
                result = BehaviourTreeStatus.Success;
        }

        return result;
    }

    /// <summary>
    /// Checks if the current character has TU greater than or equal to the given amount
    /// </summary>
    //private bool WithinCost(float cost)
    //{
    //    float timeUnits = current.Stats.CurrentTimeUnits;
    //    bool withinCost = timeUnits >= cost;
    //    return withinCost;
    //}

    private bool AbilityWithinCost()
    {
        float timeUnits = current.Stats.CurrentTimeUnits;
        bool withinCost = timeUnits >= ChosenAbility.Cost;
        return withinCost;
    }

    private bool MoveAndAbilityWithinCost()
    {
        float timeUnits = current.Stats.CurrentTimeUnits;
        bool withinCost = timeUnits >= ChosenAbility.Cost + path.Cost;
        return withinCost;
    }

    /// <summary>
    /// Moves the current character along the chosen path.
    /// </summary>
    /// <returns>Success when the character has finished moving along the path, RUNNING otherwise</returns>
    private BehaviourTreeStatus FollowPath()
    {
        // Result by default is RUNNING rather than failure
        BehaviourTreeStatus result = BehaviourTreeStatus.Running;

        if (moveOrderGiven
            && IsIdle())
        {
            moveOrderGiven = false;
            result = BehaviourTreeStatus.Success;
        }

        else
        {
            current.Move(path.Truncate(current.Stats.CurrentTimeUnits));
            moveOrderGiven = true;
        }

        return result;
    }

    /// <summary>
    /// Orders the current character to attack the chosen target with the chosen ability
    /// </summary>
    private BehaviourTreeStatus UseAbility()
    {
        // Result by default is RUNNING rather than failure
        BehaviourTreeStatus result = BehaviourTreeStatus.Running;

        if (abilityOrderGiven
            && IsIdle())
        {
            abilityOrderGiven = false;
            result = BehaviourTreeStatus.Success;
        }

        else
        {
            current.UseAbility(ChosenAbility, target.Tile);
            abilityOrderGiven = true;
        }

        return result;
    }
}
