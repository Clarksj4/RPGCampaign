using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TurnBased;

public abstract class Player : MonoBehaviour
{
    public Character Current { get; private set; }
    public List<Character> Allies { get; private set;}
    public bool IsTurn { get { return turnSystem.Current.Controller == this; } }

    protected TurnSystem turnSystem;
    protected HexGrid grid;

    protected virtual void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        grid = FindObjectOfType<HexGrid>();

        // Get list of all characters this player controls
        Allies = GetComponentsInChildren<Character>().ToList();

        Current = Allies[0];
    }

    public void AddAlly(Character actor)
    {
        Allies.Add(actor);
    }

    public void RemoveAlly(Character actor)
    {
        Allies.Remove(actor);
    }

    public virtual void PawnStart(IPawn<float> pawn)
    {
        Current = (Character)pawn;
    }
}
