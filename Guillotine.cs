using Godot;
using System;
using System.Linq;

public partial class Guillotine : Area2D
{
    private Document _document;
    public void _on_body_entered(Node2D body)
    {
        if(body is not Document document) return;
        _document = document;
    }
    
    public void _on_body_exited(Node2D body)
    {
        _document = null;
    }


    public override void _UnhandledInput(InputEvent input)
    {
        if(input is InputEventKey inputKey)
        {
            if(inputKey.Keycode == Key.Enter && inputKey.Pressed)
            {
                var globalPolygon = GetChild<Polygon2D>(1).Polygon.Select(ToGlobal).ToArray();
                _document?.Slice(globalPolygon);
            }
        }
    }
}
