using Godot;

public class CoordinateUtilities
{
    public static Vector2I WorldToTexture(Vector2 point)
    {
        return (Vector2I)(((point + new Vector2(198.5f, 280.5f))*2f).Floor());
    }
}