using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public List<Character> Actors;

    public Character Current { get { return Actors.First(); } }

    private void Awake()
    {
        Actors = FindObjectsOfType<Character>().ToList();
        Order();
    }

    private void Start()
    {
        // Tell next character to activate!
        Actors.First().Controller.Activate(Actors.First());
    }

    /// <summary>
    /// Add the given character to the initiative order
    /// </summary>
    public void Add(Character actor)
    {
        // Find first actor with greater initiative
        int i = 0;
        for (i = 0; i < Actors.Count; i++)
        {
            if (Actors[i].Stats.Initiative > actor.Stats.Initiative)
                break;
        }

        // Insert in front of actor with greater initiative
        Actors.Insert(i, actor);
    }

    /// <summary>
    /// Adds the given character to the initiative order so that its turn is immediately after the character that is currently acting
    /// </summary>
    public void AddNext(Character actor)
    {
        // Add after first character in list
        Actors.AddNext(actor);
    }

    /// <summary>
    /// Pushes the given character the given number of places forward / back the iniative order. The new position of the character is
    /// clamped to the bounds of the initiative order. For example, pushing 10 places in an order with 5 characters will place the given
    /// character at the end of the order; i.e. position 5.
    /// </summary>
    public void Push(Character actor, int places)
    {
        Actors.Move(actor, places);
    }

    /// <summary>
    /// Moves the given character to the place in the initiative order immediately AFTER the given reference character
    /// </summary>
    public void MoveAfter(Character actor, Character referenceCharacter)
    {
        Actors.MoveAfter(actor, referenceCharacter);
    }

    /// <summary>
    /// Moves the given character to the place in the initiative order immediately BEFORE the given reference character
    /// </summary>
    public void MoveBefore(Character actor, Character referenceCharacter)
    {
        Actors.MoveBefore(actor, referenceCharacter);
    }

    /// <summary>
    /// Randomly orders all characters in the initiative order
    /// </summary>
    public void Shuffle()
    {
        Actors.Shuffle();
    }

    /// <summary>
    /// Orders all characters in the initiative order ascending according to their initiative value
    /// </summary>
    public void Order()
    {
        Actors.OrderBy(c => c.Stats.Initiative);
    }

    /// <summary>
    /// Cycles the initiative order by one place. All characters in the order are advanced one place, the character at the 
    /// front of the order is sent to the back. The next character in front is activated
    /// </summary>
    public void Cycle()
    {
        // Save temp reference to character that has just finished turn
        Character finished = Actors.First();

        // Remove character from list and add to end
        Actors.RemoveAt(0);
        Actors.Add(finished);

        // Tell next character to activate!
        Actors.First().Controller.Activate(Actors.First());
    }
}
