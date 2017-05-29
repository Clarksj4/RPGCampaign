using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TurnBased;
using TileMap;

public abstract class Player : MonoBehaviour
{
    public Character Current { get; protected set; }
    public List<Character> Allies { get; protected set;}
    public bool IsTurn { get { return turnSystem.Current.Controller == this; } }

    protected TurnSystem turnSystem;
    protected ITileMap<Character> grid;

    protected virtual void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        grid = FindObjectOfType<HexGrid>();

        // Get list of all characters this player controls
        Allies = GetComponentsInChildren<Character>().ToList();

        //Current = Allies[0];
    }

    public void AddAlly(Character actor)
    {
        Allies.Add(actor);
    }

    public void RemoveAlly(Character actor)
    {
        Allies.Remove(actor);
    }

    public virtual void PawnStart(ITurnBased<float> pawn)
    {
        Current = (Character)pawn;
    }

    public abstract void PawnDie(Character pawn);
}
