using System.Linq;
using System.Collections.Generic;
using TurnBased;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Turn Based/Turn System")]
public class TurnSystem : MonoBehaviour
{
    public bool BeginOnLoad;

    [SerializeField]
    public TurnEvent TurnEnding;
    [SerializeField]
    public TurnEvent TurnStarting;
    [SerializeField]
    public UnityEvent CycleComplete;
    [SerializeField]
    public TurnEvent OrderChanged;

    public TurnBasedEntity Current { get { return order.Current as TurnBasedEntity; } }
    public IEnumerable<TurnBasedEntity> Order { get { return order.Select(i => i as TurnBasedEntity); } }

    private TurnOrder<float> order = new TurnOrder<float>();

    void Start()
    {
        if (BeginOnLoad)
            EndTurn();
    }

    /// <summary>
    /// End the current entities turn, progress to the next entity, restarting the cycle if it is complete
    /// </summary>
    public void EndTurn()
    {
        // Only do a thing if the order is not empty
        if (order.Count > 0)
        {
            // Notify current object's turn has ended
            if (Current != null)
                TurnEnding.Invoke(Current);

            // Move to next object if there is any
            bool isMore = order.MoveNext();

            // If there are no more, then a complete cycle is complete 
            if (!isMore)
            {
                // Notify a complete turn cycle is finished
                CycleComplete.Invoke();
                order.MoveNext(); // Start the cycle again
            }

            // Notify turn has started
            TurnStarting.Invoke(Current);
        }
    }

    public bool Contains(TurnBasedEntity entity)
    {
        return order.Contains(entity);
    }

    /// <summary>
    /// Insert an entity into the order
    /// </summary>
    public void Insert(TurnBasedEntity entity)
    {
        // Insert wrapper object into order
        order.Insert(entity);

        // Notify that an object has been inserted
        OrderChanged.Invoke(entity);
    }

    // [PLACHOLDER]: TODO remove this test code
    public void Add()
    {
        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        primitive.transform.SetParent(transform);
        var entity = primitive.AddComponent<TurnBasedEntity>();
    }

    // [PLACHOLDER]: TODO remove this test code
    public void Remove()
    {
        TurnBasedEntity obj = FindObjectOfType<TurnBasedEntity>();
        if (obj != null)
        {
            Remove(obj);
            Destroy(obj.gameObject);
        }
    }

    /// <summary>
    /// Remove an entity from the order. If the current entity is removed, the turn order progresses to the next entity
    /// </summary>
    public void Remove(TurnBasedEntity entity, bool moveNextIfCurrent = true)
    {
        // Check if the current item is being removed
        bool currentRemoved = entity == Current;

        // Remove wrapper object from order
        order.Remove(entity);

        // Notify that an object has been removed
        OrderChanged.Invoke(entity);

        // If the current item was removed, progress to next item's turn
        if (currentRemoved &&
            moveNextIfCurrent && 
            order.Count > 0)
            EndTurn();
    }

    /// <summary>
    /// Update the priority of the given entity; reordering entities
    /// </summary>
    public void UpdatePriority(TurnBasedEntity entity)
    {
        // Update wrapper priority in order
        order.UpdatePriority(entity);

        // Notify that an object has had its priority changed
        OrderChanged.Invoke(entity);
    }
}
