using Godot;

public class CoordinateUtilities
{
    public static Vector2I WorldToTexture(Vector2 point)
    {
        return (Vector2I)((point + new Vector2(230.4f, 345.6f))*1.72f);
    }
}