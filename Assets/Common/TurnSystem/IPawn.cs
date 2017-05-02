using System;

namespace TurnBased
{
    /// <summary>
    /// Interface for object that operates in a turn based manner
    /// </summary>
    public interface IPawn : IComparable<IPawn>
    {
        /// <summary>
        /// The object that controls this pawn
        /// </summary>
        IPawnController Controller { get; }

        /// <summary>
        /// The pawn's turn is starting
        /// </summary>
        void TurnStart();

        /// <summary>
        /// The pawn's turn is ending
        /// </summary>
        void TurnEnd();
    }
}
