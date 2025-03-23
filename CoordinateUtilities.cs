using Godot;

public class CoordinateUtilities
{
    public static Vector2I WorldToTexture(Vector2 point)
    {
        return (Vector2I)((point + new Vector2(128, 192))*2.5f);
    }
}