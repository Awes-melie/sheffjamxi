using Godot;
using System;
using System.Diagnostics;

public partial class Document : RigidBody2D
{
	[Export]
	public float MovementScalar{get; set;}

	private PackedScene _documentScene = ResourceLoader.Load<PackedScene>("res://Document.tscn");

	private Vector2 _mouseGrabbedPosition;
	private bool _mouseOver;
	private bool _grabbing;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public void _on_mouse_entered() => _mouseOver = true;
	public void _on_mouse_exited() => _mouseOver = false;

	public override void _UnhandledInput(InputEvent eventArgs)
	{
		if (eventArgs is InputEventMouseButton mouseEvents)
		{
			if (!_mouseOver) return;

			_grabbing = mouseEvents.Pressed;
			
			if (!_grabbing) return;

			var mousePos = GetGlobalMousePosition();
			_mouseGrabbedPosition = ToLocal(mousePos);
		} 
		else if (eventArgs is InputEventMouseMotion)
		{
			if(! _grabbing) return;

			var mousePos = GetGlobalMousePosition();
			var deltaMovement = ToLocal(mousePos) - _mouseGrabbedPosition;
			Translate(deltaMovement);   
		}
	}

	public void Slice()
	{
		var instance = _documentScene.Instantiate();

		var newScale = new Vector2(1, 0.5f);
		GetParent().AddChild(instance);
		
		((Node2D)instance).Scale = Scale;
		
		((Node2D)instance).ApplyScale(newScale);
		ApplyScale(newScale);
	}
}
