using HexMapPathfinding;

public delegate void CharacterMovementEventHandler(object sender, CharacterMovementEventArgs e);

public class CharacterMovementEventArgs
{
    public HexPath Path { get; private set; }

    public CharacterMovementEventArgs(HexPath path)
    {
        Path = path;
    }
}
