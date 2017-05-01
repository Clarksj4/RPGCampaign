using System;

namespace TurnBased
{
    public interface ITurnBasedPawn : IComparable<ITurnBasedPawn>
    {
        ITurnBasedController Controller { get; }

        void TurnStart();
        void TurnEnd();
    }
}
