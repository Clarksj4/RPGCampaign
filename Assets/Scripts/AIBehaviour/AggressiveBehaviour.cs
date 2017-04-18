using System;
using System.Collections.Generic;
using System.Linq;
using FluentBehaviourTree;
using UnityEngine;
using HexMapPathfinding;

public class AggressiveBehaviour : IBehaviourStrategy
{
    private IBehaviourTreeNode behaviourTree;

    private Character current;
    private Character target;
    private Spell spell;
    private HexPath path;
    private bool setAlongPath = false;
    private bool toldToAttack = false;

    public AggressiveBehaviour()
    {
        BehaviourTreeBuilder builder = new BehaviourTreeBuilder();
        behaviourTree = builder
            .Sequence("Sequence")

                // TODO: subtree more intelligently picks a spell for the situation
                .Do("Pick spell", t => PickSpell())
                .Sequence("Sequence")

                    // TODO: subtree more intelligently picks a target
                    .Do("Find target", t => FindTarget())
                    .Sequence("Sequence")
                        .Selector("Range")

                            // If not in range
                            .Condition("In range?", t => InRange())

                            // Move into range if enough TU 
                            .Sequence("Get in range")

                                // Find a path to a cell that is within range of the chosen attack
                                .Do("Find path to in range position", t => FindPath())

                                // Enough TU to move AND attack
                                .Condition("Enough TU for move and attack?", t => WithinCost())

                                // Do the move
                                .Do("Move into range", t => FollowPath())
                            .End()
                        .End()

                        // Cast the chosen spell / attack if enough TU
                        .Sequence("Cast spell")
                            .Condition("Enough TU for attack?", t => current.Stats.CurrentTimeUnits >= spell.Cost)
                            .Do("Attack!", t => Cast())
                        .End()
                    .End()
                .End()
            .End()
            .Build();
    }

    public void Activate(Character current)
    {
        this.current = current;

        // Reset behaviour tree variables
        target = null;
        spell = null;
        path = null;
        setAlongPath = false;
        toldToAttack = false;
    }

    public BehaviourTreeStatus Update()
    {
        return behaviourTree.Tick(new TimeData(Time.deltaTime));
    }

    /// <summary>
    /// Picks the first spell of the current character's spells.
    /// </summary>
    /// <returns>Success if the character has atleast one spell, failure otherwise</returns>
    private BehaviourTreeStatus PickSpell()
    {
        BehaviourTreeStatus result = BehaviourTreeStatus.Failure;

        // If the current character has atleast one attack, use the first one
        if (current.Spells.Length > 0)
        {
            spell = current.Spells[0];
            result = BehaviourTreeStatus.Success;
        }

        // Successful if the current character has atleast one attack
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
        target = enemies[0];

        // If found an enemey
        if (target != null)
            result = BehaviourTreeStatus.Success;

        return result;
    }

    /// <summary>
    /// Checks if the chosen attack is in range of the chosen target
    /// </summary>
    /// <returns>True if the attack is currently in range</returns>
    private bool InRange()
    {
        bool inRange = spell.InRange(current.Cell, target.Cell);
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

        // Get area around target which would put AI character in range for attack
        ICollection<Step> area = Pathfind.Area(target.Cell, spell.MaximumRange, spell.Traverser);

        // Find a path from the current characters cell to the quickest to reach cell that is in range of the target for 
        // the given attack
        path = Pathfind.ToArea(current.Cell, area.Select(s => s.Cell), current.Stats.Traverser);

        // Is the path legit?
        if (path != null && path.Count >= 2)
            result = BehaviourTreeStatus.Success;

        return result;
    }

    /// <summary>
    /// Checks if the current character has enough time units to traverse the path AND then perform the chosen attack
    /// </summary>
    /// <returns>True if the current character has enough time units to move and attack</returns>
    private bool WithinCost()
    {
        bool withinCost = current.Stats.CurrentTimeUnits >= path.Cost + spell.Cost;
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

    /// <summary>
    /// Orders the current character to attack the chosen target with the chosen spell
    /// </summary>
    /// <returns>Success when the character has finished attacking the target, RUNNING otherwise</returns>
    private BehaviourTreeStatus Cast()
    {
        // Result by default is RUNNING rather than failure
        BehaviourTreeStatus result = BehaviourTreeStatus.Running;

        if (!toldToAttack)
        {
            current.Cast(spell, target.Cell);
            toldToAttack = true;
        }

        else if (!current.IsCasting)
        {
            toldToAttack = false;
            result = BehaviourTreeStatus.Success;
        }

        return result;
    }
}
