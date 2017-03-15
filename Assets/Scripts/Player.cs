using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Player : MonoBehaviour
{
    public TurnSystem TurnSystem;

    private List<Character> Actors;

    private void Awake()
    {
        // Get list of all characters this player controls
        Actors = GetComponentsInChildren<Character>().ToList();
    }

    private void Start()
    {
        // Tell each character that this player is its controller
        foreach (Character actor in Actors)
            actor.Controller = this;
    }

    public void Add(Character actor)
    {
        Actors.Add(actor);
    }

    public void Remove(Character actor)
    {
        Actors.Remove(actor);
    }

    public abstract void Activate(Character actor);

    public abstract void EndTurn(Character actor);
}
