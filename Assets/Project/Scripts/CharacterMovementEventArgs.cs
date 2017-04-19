using Pathfinding;

public delegate void CharacterMovementEventHandler(object sender, CharacterMovementEventArgs e);

public class CharacterMovementEventArgs
{
    public Path Path { get; private set; }

    public CharacterMovementEventArgs(Path path)
    {
        Path = path;
    }
}
