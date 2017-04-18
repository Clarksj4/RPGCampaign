using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Player : MonoBehaviour
{
    protected Character current;
    protected List<Character> characters;

    public List<Character> Characters { get { return characters; } }
    public Character Current { get { return current; } }
    public bool IsTurn { get { return turnSystem.CurrentPlayer == this; } }

    private TurnSystem turnSystem;

    protected virtual void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();

        // Get list of all characters this player controls
        characters = GetComponentsInChildren<Character>().ToList();
    }

    protected virtual void Start()
    {
        foreach (Character actor in characters)
            actor.Controller = this;
    }

    public void Add(Character actor)
    {
        characters.Add(actor);
    }

    public void Remove(Character actor)
    {
        characters.Remove(actor);
    }

    public virtual void Activate(Character actor)
    {
        current = actor;
    }

    public virtual void EndTurn()
    {
        turnSystem.EndTurn();
    }
}
