using System.Collections.Generic;
using System;
using UnityEngine;

public static class ListExtension
{
    /// <summary>
    /// Remove and return the first item in the list
    /// </summary>
    public static T PopFirst<T>(this LinkedList<T> list)
    {
        LinkedListNode<T> node = list.First;
        list.RemoveFirst();
        return node.Value;
    }

    /// <summary>
    /// Shuffle the list according to Fisher-Yates shuffle
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        for (var i = 0; i < list.Count; i++)
            list.Swap(i, UnityEngine.Random.Range(i, list.Count - 1));
    }

    /// <summary>
    /// Swap the elements in the list at the given indices
    /// </summary>
    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    /// <summary>
    /// Adds the item immediately after the first item in the list
    /// </summary>
    public static void AddNext<T>(this IList<T> list, T item)
    {
        list.Insert(1, item);
    }

    /// <summary>
    /// Moves the given item the given number of places forward / back the list. The new position of the item is clamped to the bounds of 
    /// the list. For example, moving 10 places in a list with 5 item will place the given item at the end of the list; i.e. position 5.
    /// </summary>
    public static void Move<T>(this IList<T> list, T item, int places)
    {
        // Curent index of item in list
        int index = list.IndexOf(item);
        if (index < 0)
            throw new ArgumentException("Item not present in list");

        // Calculate new position and insert item there
        int newPosition = index + places;
        newPosition = Mathf.Clamp(newPosition, 0, list.Count);
        list.Insert(newPosition, item);

        // If item was pushed forwards in order, then original index will have been incremented
        if (places < 0)
            list.RemoveAt(index + 1);     // Remove from original index

        // Otherwise, remove at original index
        else
            list.RemoveAt(index);
    }

    /// <summary>
    /// Moves the first instance of the given item to the place in the list immediately AFTER the first instance of the given reference 
    /// item
    /// </summary>
    public static void MoveAfter<T>(this IList<T> list, T item, T referenceItem)
    {
        // Remove from current position in list
        if (!list.Remove(item))
            throw new ArgumentException("Item not present in list");   // If it wasn't in the list

        // Get position of reference item
        int beforeIndex = list.IndexOf(referenceItem);
        if (beforeIndex < 0)
            throw new ArgumentException("Reference item not present in list");   // If it wasn't in the list

        // Insert after reference item
        list.Insert(beforeIndex + 1, item);
    }

    /// <summary>
    /// Moves the first instance of the given item to the place in the list immediately BEFORE the first instance of the given reference 
    /// item
    /// </summary>
    public static void MoveBefore<T>(this IList<T> list, T item, T referenceItem)
    {
        // Remove from current position in list
        if (!list.Remove(item))
            throw new ArgumentException("Item not present in list");   // If it wasn't in the list

        // Get position of reference item
        int afterIndex = list.IndexOf(referenceItem);
        if (afterIndex < 0)
            throw new ArgumentException("Reference item not present in list");   // If it wasn't in the list

        // Insert before reference item
        list.Insert(afterIndex, item);
    }
}
