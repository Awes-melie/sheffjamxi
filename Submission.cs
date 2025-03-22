using Godot;
using System;

public partial class Submission : Area2D
{

    public void _on_body_entered(Node2D body) {
        if (body is Document document) {
            document.ConstantForce = (new Vector2(0,-10000));
        }
    }

    public void _on_body_exited(Node2D body) {
        if (body is Document document) {
            document.ConstantForce = (new Vector2(0,0));
        }
    }
}
