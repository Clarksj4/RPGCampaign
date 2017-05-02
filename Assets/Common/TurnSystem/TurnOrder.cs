using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TurnBased
{
    /// <summary>
    /// Stores and cycles pawns in priority order based upon their implementation of the IComparable interface
    /// </summary>
    public class TurnOrder : IEnumerable<IPawn>
    {
        /// <summary>
        /// The whose turn it currently is. Returns null if the current pawn was removed from the order
        /// </summary>
        public IPawn Current
        {
            get
            {
                if (currentToBeRemoved ||   // If Current was removed from list, don't return its ref
                    currentNode == null)    // Current node not yet set
                    return null;

                // Get pawn from linked list node
                return currentNode.Value;
            }
        }

        private LinkedList<IPawn> pawns;
        private LinkedListNode<IPawn> currentNode;
        private bool currentToBeRemoved;

        /// <summary>
        /// An empty turn order
        /// </summary>
        public TurnOrder()
        {
            pawns = new LinkedList<IPawn>();
            currentNode = null;
            currentToBeRemoved = false;
        }

        /// <summary>
        /// Inserts the pawn into the turn order based upon its associated IComparable
        /// </summary>
        public void Insert(IPawn pawn)
        {
            // Can't insert null pawn
            if (pawn == null)
                throw new ArgumentNullException("Pawn cannot be null");

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

        /// <summary>
        /// Removes the pawn from the turn order
        /// </summary>
        public bool Remove(IPawn pawn)
        {
            // Can't remove null pawn
            if (pawn == null)
                throw new ArgumentNullException("Pawn cannot be null");

            // Find pawn's node incase its the current node
            LinkedListNode<IPawn> node = pawns.Find(pawn);

            if (node == null)
                return false;

            // If current pawn is being removed, marked it as removed
            if (node == currentNode)
                currentToBeRemoved = true;
            else
                pawns.Remove(node);

            // Pawn successfully removed
            return true;
        }

        /// <summary>
        /// Updates the pawns position in the turn order based upon its associated IComparable. 
        /// </summary>
        public void UpdatePriority(IPawn pawn)
        {
            // Can't update pawn marked for removal
            if (currentToBeRemoved && currentNode.Value == pawn)
                throw new ArgumentException("Order does not contain pawn");

            // Remove from order
            bool removed = Remove(pawn);

            // Can't update pawn if it doesn't exists in order
            if (removed == false)
                throw new ArgumentException("Order does not contain pawn");

            // Re-insert into order in correct position
            Insert(pawn);
        }

        /// <summary>
        /// Move to the next pawn in the turn order. Notifies the next pawn of its turn starting.
        /// </summary>
        /// <returns>True if there is another pawn in the order who has not had its turn yet during this cycle</returns>
        public bool MoveNext()
        {
            // Can't move to next pawn in order if there is none
            if (pawns.Count == 0)
                throw new InvalidOperationException("Order is empty");

            // Notify current of turn end
            EndCurrent();

            // Remove current node if its been marked
            DeferredRecycle();

            // Move to next pawn in order
            bool isMore = Cycle();

            // Notify current of turn start
            StartCurrent();

            // True if there are more pawns in the order who have not had their turn during this cycle
            return isMore;
        }

        public IEnumerator<IPawn> GetEnumerator()
        {
            // Don't return the node that is marked for removal
            if (currentToBeRemoved)
                return pawns.Where(p => p != currentNode.Value).GetEnumerator();

            else
                return pawns.GetEnumerator();
        }

        /// <summary>
        /// Informs the current pawn that its turn has ended
        /// </summary>
        void EndCurrent()
        {
            // If thing has been removed from order, do not notify it of turn end
            if (Current != null)
                Current.TurnEnd();
        }

        void DeferredRecycle()
        {
            // Remove current node if its been marked
            if (currentToBeRemoved)
                pawns.Remove(currentNode);

            // Update mark
            currentToBeRemoved = false;
        }

        /// <summary>
        /// Proceed to next pawn in order. 
        /// </summary>
        /// <returns>True if there is another pawn in the order who has not had its turn yet during this cycle</returns>
        bool Cycle()
        {
            // Move to next pawn in order
            if (currentNode == null)
                currentNode = pawns.First;

            else
                currentNode = currentNode.Next;

            return currentNode != null;
        }

        /// <summary>
        /// Inform current pawn and controller that its their turn
        /// </summary>
        void StartCurrent()
        {
            if (Current != null)
            {
                // Activate current pawn...
                Current.TurnStart();

                // ...THEN tell controller pawn is active
                if (Current.Controller != null)
                    Current.Controller.PawnStart(Current);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Don't return the node that is marked for removal
            if (currentToBeRemoved)
                return pawns.Where(p => p != currentNode.Value).GetEnumerator();

            else
                return pawns.GetEnumerator();
        }
    }
}
