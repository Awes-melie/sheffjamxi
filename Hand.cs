using Godot;
using System;

public partial class Hand : StaticBody2D
{
    public override void _PhysicsProcess(double delta)
    {
        var deltaMovement = GetGlobalMousePosition() - Position;
		Translate(deltaMovement);
    }
}
