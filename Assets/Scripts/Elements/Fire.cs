using UnityEngine;

public class Fire : Element
{
    public Fire()
    {
        type = ElementType.Fire;
        Colour = Color.red;

        StrengthModifiers = new float[4];
        StrengthModifiers[(int)ElementType.Air] = 1f;
        StrengthModifiers[(int)ElementType.Earth] = 0.5f;
        StrengthModifiers[(int)ElementType.Fire] = -0.25f;
        StrengthModifiers[(int)ElementType.Water] = 0.25f;

        ElementCapacityModifiers = new float[4];
        ElementCapacityModifiers[(int)ElementType.Air] = 0.25f;
        ElementCapacityModifiers[(int)ElementType.Earth] = 0.5f;
        ElementCapacityModifiers[(int)ElementType.Fire] = 1f;
        ElementCapacityModifiers[(int)ElementType.Water] = 0.5f;
    }
}