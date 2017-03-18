using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Player : MonoBehaviour
{
    public GameManager GameManager;

    protected Character current;
    protected List<Character> Actors;

    public bool IsTurn { get { return GameManager.CurrentPlayer == this; } }

    private void Awake()
    {
        // Get list of all characters this player controls
        Actors = GetComponentsInChildren<Character>().ToList();
    }

    private void Start()
    {
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

    public virtual void Activate(Character actor)
    {
        current = actor;
    }

    public abstract void EndTurn(Character actor);
}
