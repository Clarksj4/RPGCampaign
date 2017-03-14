using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    public List<Character> Actors;
    public float TurnThreshold = 100f;

    private List<float> actorInitiative;
    private List<int> nextActorsIndex;

    public Character Current { get { return Actors.First(); } }

    private void Awake()
    {
        actorInitiative = new List<float>(Actors.Count);
    }

    public void AddActor(Character actor)
    {
        Actors.Add(actor);
        actorInitiative.Add(0f);
    }

    public void AddAsNextActor(Character actor)
    {
        AddActor(actor);
        nextActorsIndex.Insert(1, Actors.Count - 1);
    }

    public void Next()
    {
        // If there are actors in the queue...
        if (nextActorsIndex.Count > 0)
        {
            // Reduce initiative count by threshold so values do not become large
            actorInitiative[0] -= TurnThreshold;

            // Remove from order as their turn is complete
            nextActorsIndex.RemoveAt(0);
        }

        // Otherwise, populate the queue
        if (nextActorsIndex.Count == 0)
            nextActorsIndex = CreateActorQueue(1, actorInitiative);
    }

    public IEnumerable<Character> GetNextActors(int n)
    {
        // Create temporary list of initiatives, so current initiative values are not affected
        List<float> initiative = new List<float>(actorInitiative);

        // Create actor index queue and convert to actors
        return CreateActorQueue(n, initiative).Select(i => Actors[i]);
    }

    List<int> CreateActorQueue(int count, List<float> initiative)
    {
        List<int> actorIndexQueue = new List<int>();

        // While queue does not have required amount of indices in it
        while (actorIndexQueue.Count < count)
        {
            // Loop through ALL characters...
            for (int i = 0; i < Actors.Count; i++)
            {
                // Add their initiative stat to their stored initiative value
                initiative[i] += Actors[i].Stats.Initiative;

                // If this value now exceeds the turn threshold..
                if (initiative[i] >= TurnThreshold)
                    actorIndexQueue.Add(i);                     // Add character's index to actor queue (they get a turn!)
            }
        }

        return actorIndexQueue;
    }
}
