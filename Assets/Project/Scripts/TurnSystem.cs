using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TurnBased;

public class TurnSystem : MonoBehaviour
{
    public TurnOrder<float> Order { get; private set; }
    public Character Current { get { return (Character)Order.Current; } }

    void Awake()
    {
        Order = new TurnOrder<float>();

        Character[] characters = FindObjectsOfType<Character>();
        foreach (Character character in characters)
            Order.Insert(character);
    }

    void Start()
    {
        Order.MoveNext();
    }

    public void Remove(Character character)
    {
        Order.Remove(character);
    }

    public void EndTurn()
    {
        bool isMore = Order.MoveNext();

        // Currently does nothing at the end of a complete cycle...
        if (!isMore)
            Order.MoveNext(); // Start the cycle again
    }
}
