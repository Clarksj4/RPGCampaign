using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public List<Character> Characters { get; private set; }
    public List<Player> Players { get; private set; }
    public Character CurrentCharacter { get { return Characters.First(); } }
    public Player CurrentPlayer { get { return CurrentCharacter.Controller; } }

    void Awake()
    {
        Characters = FindObjectsOfType<Character>().ToList();
        Players = FindObjectsOfType<Player>().ToList();
        Reorder();
    }

    void Start()
    {
        CurrentCharacter.BeginTurn();
    }

    public void EndTurn()
    {
        Cycle();
        CurrentCharacter.BeginTurn();
    }

    /// <summary>
    /// Add the given character to the initiative order
    /// </summary>
    public void AddInOrder(Character character)
    {
        // Find first actor with greater initiative
        int i = 0;
        for (i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].Stats.Initiative > character.Stats.Initiative)
                break;
        }

        // Insert in front of actor with greater initiative
        Characters.Insert(i, character);
    }

    /// <summary>
    /// Adds the given character to the initiative order so that its turn is immediately after the character that is currently acting
    /// </summary>
    public void AddNext(Character character)
    {
        // Add after first character in list
        Characters.AddNext(character);
    }

    /// <summary>
    /// Pushes the given character the given number of places forward / back the iniative order. The new position of the character is
    /// clamped to the bounds of the initiative order. For example, pushing 10 places in an order with 5 characters will place the given
    /// character at the end of the order; i.e. position 5.
    /// </summary>
    public void Push(Character character, int places)
    {
        Characters.Move(character, places);
    }

    /// <summary>
    /// Moves the given character to the place in the initiative order immediately AFTER the given reference character
    /// </summary>
    public void MoveAfter(Character character, Character referenceCharacter)
    {
        Characters.MoveAfter(character, referenceCharacter);
    }

    /// <summary>
    /// Moves the given character to the place in the initiative order immediately BEFORE the given reference character
    /// </summary>
    public void MoveBefore(Character character, Character referenceCharacter)
    {
        Characters.MoveBefore(character, referenceCharacter);
    }

    /// <summary>
    /// Randomly orders all characters in the initiative order
    /// </summary>
    public void Shuffle()
    {
        Characters.Shuffle();
    }

    /// <summary>
    /// Orders all characters in the initiative order ascending according to their initiative value
    /// </summary>
    public void Reorder()
    {
        Characters = Characters.OrderByDescending(c => c.Stats.Initiative).ToList();
    }

    /// <summary>
    /// Cycles the initiative order by one place. All characters in the order are advanced one place, the character at the 
    /// front of the order is sent to the back. The next character in front is activated
    /// </summary>
    private void Cycle()
    {
        // Save temp reference to character that has just finished turn
        Character finished = Characters.First();

        // Remove character from list and add to end
        Characters.RemoveAt(0);
        Characters.Add(finished);
    }
}
