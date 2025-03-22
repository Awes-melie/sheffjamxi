using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

public partial class Hand : StaticBody2D
{   
    public ToolType toolType = ToolType.HAND;

    public static Hand Instance {get; set;}

    public Hand()
    {
        Instance ??= this;
    }

    Texture2D handTex = GD.Load<Texture2D>("res://hand.png");
    Texture2D handGrabTex = GD.Load<Texture2D>("res://handgrab.png");
    Texture2D penTex = GD.Load<Texture2D>("res://pen.png");
    Texture2D pencilTex = GD.Load<Texture2D>("res://pencil.png");
    public override void _PhysicsProcess(double delta)
    {
        var deltaMovement = GetGlobalMousePosition() - Position;
		Translate(deltaMovement);
    }


    public void _on_pen_toggled(bool holdingPen) {
        if (holdingPen) 
        {
            toolType = ToolType.PEN; 
            GetChild<Sprite2D>(1).Texture = penTex;
        } else {
            toolType = ToolType.HAND;
            GetChild<Sprite2D>(1).Texture = handTex;
        }

    }

    public void _on_pencil_toggled(bool holdingPencil) {
        if (holdingPencil) 
        {
            toolType = ToolType.PENCIL;
            GetChild<Sprite2D>(1).Texture = pencilTex;
        } else {
            toolType = ToolType.HAND;
            GetChild<Sprite2D>(1).Texture = handTex;
        }

    }

    public override void _UnhandledInput(InputEvent eventArgs)
	{   
        if (eventArgs is InputEventMouseButton mouseEvents)
		{
            if (!mouseEvents.Pressed || mouseEvents.ButtonIndex != MouseButton.Left) 
            {   
                if (toolType == ToolType.HAND) {GetChild<Sprite2D>(1).Texture = handTex;}
                return;
                }

            GetChild<Sprite2D>(1).Texture = handGrabTex;

            if (toolType == ToolType.HAND) return;
                
            GetChild<Sprite2D>(1).Texture = penTex;

            if (toolType == ToolType.PEN) return;

            GetChild<Sprite2D>(1).Texture = pencilTex;
            
            if (toolType == ToolType.PENCIL) return;

            }
        }
        
        
    }
