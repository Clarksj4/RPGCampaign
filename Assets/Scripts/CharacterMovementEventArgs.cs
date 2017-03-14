using System.Collections.Generic;
using UnityEngine;

public delegate void CharacterMovementEventHandler(object sender, CharacterMovementEventArgs e);

public class CharacterMovementEventArgs
{
    public List<Step> Path { get; private set; }

    public CharacterMovementEventArgs(List<Step> path)
    {
        Path = path;
    }
}
