using UnityEngine;

public class Earth : Element
{
    public Earth()
    {
        type = ElementType.Earth;
        Colour = Color.green;

        StrengthModifiers = new float[4];
        StrengthModifiers[(int)ElementType.Air] = 0.25f;
        StrengthModifiers[(int)ElementType.Earth] = -0.25f;
        StrengthModifiers[(int)ElementType.Fire] = 0.5f;
        StrengthModifiers[(int)ElementType.Water] = 1f;

        ElementCapacityModifiers = new float[4];
        ElementCapacityModifiers[(int)ElementType.Air] = 0.5f;
        ElementCapacityModifiers[(int)ElementType.Earth] = 1f;
        ElementCapacityModifiers[(int)ElementType.Fire] = 0.5f;
        ElementCapacityModifiers[(int)ElementType.Water] = 0.25f;
    }
}
