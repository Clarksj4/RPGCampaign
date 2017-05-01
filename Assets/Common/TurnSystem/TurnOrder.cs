using System;
using System.Collections;
using System.Collections.Generic;

namespace TurnBased
{
    public class TurnOrder : IEnumerable<ITurnBasedPawn>
    {
        // Nodes stored in order of priority
        // Node order manupulatable by updating a node's priority
        // Node order must be iterable (to display upcomming turns on UI)

        public ITurnBasedPawn Current
        {
            get
            {
                // If current was removed from list, don't return its ref
                if (removed)
                    return null;

                return current.Value;
            }
        }

        private LinkedList<ITurnBasedPawn> pawns;
        private LinkedListNode<ITurnBasedPawn> current;
        private bool removed;

        public TurnOrder()
        {
            pawns = new LinkedList<ITurnBasedPawn>();
            current = null;
            removed = false;
        }

        public void Insert(ITurnBasedPawn pawn)
        {
            // Can't have duplicates in order
            if (pawns.Contains(pawn))
                throw new ArgumentException("Order already contains pawn");

            // First pawn to be inserted
            if (pawns.Count == 0)
                pawns.AddFirst(pawn);

            else
            {
                var walker = pawns.First;

                // Walk until finding a smaller node
                while (walker != null && 
                       pawn.CompareTo(walker.Value) < 0)
                    walker = walker.Next;

                // Add to end of order
                if (walker == null)
                    pawns.AddLast(pawn);

                // Add in front of smaller node
                else
                    pawns.AddBefore(walker, pawn);
            }
        }

        public void Remove(ITurnBasedPawn pawn)
        {
            // Can't insert null pawn
            if (pawn == null)
                throw new ArgumentNullException("Pawn cannot be null");

            // Find pawn's node incase its the current node
            LinkedListNode<ITurnBasedPawn> node = pawns.Find(pawn);

            // If current pawn is being removed, marked it as removed
            if (node == current)
                removed = true;

            pawns.Remove(node);
        }

        // TODO: Interate once
        public void UpdatePriority(ITurnBasedPawn pawn)
        {
            // Remove from order
            Remove(pawn);

            // Re-insert into order in correct position
            Insert(pawn);
        }

        // Returns true if there are pawns who have not had their turn during this cycle
        public bool MoveNext()
        {
            // Can't move to next pawn in order if there is none
            if (pawns.Count == 0)
                throw new InvalidOperationException("Order is empty");

            // Notify current of turn end
            EndCurrent();

            // Move to next pawn in order
            bool isMore = Cycle();

            // Notify current of turn start
            StartCurrent();

            // True if there are more pawns in the order who have not had their turn during this cycle
            return isMore;
        }

        public IEnumerator<ITurnBasedPawn> GetEnumerator()
        {
            return pawns.GetEnumerator();
        }

        void EndCurrent()
        {
            // If thing has been removed from order, do not notify it of turn end
            if (!removed && current != null)
                Current.TurnEnd();
        }

        bool Cycle()
        {
            // Move to next pawn in order
            if (current == null)
                current = pawns.First;

            else
                current = current.Next;

            // No longer applicable whether item was removed
            removed = false;

            return current != null;
        }

        void StartCurrent()
        {
            if (current != null)
            {
                // Activate current pawn...
                Current.TurnStart();

                // ...THEN tell controller pawn is active
                Current.Controller.PawnStart(Current);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pawns.GetEnumerator();
        }
    }
}
