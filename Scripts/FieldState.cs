using System;
using Godot;

public class FieldState()
{
    public FilledType FilledType { get; set; }

    public Texture2D CurrentTexture { get; set; }

    public FieldState Clone()
    {
        return new FieldState 
        {
            FilledType = FilledType,
            CurrentTexture = CurrentTexture
        };
    }

}