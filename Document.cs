using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class Document : RigidBody2D
{
	[Export]
	public float MovementScalar{get; set;}

	private PackedScene _documentScene = ResourceLoader.Load<PackedScene>("res://Document.tscn");

	private Vector2 _mouseGrabbedPosition;
	private bool _mouseOver;
	private bool _grabbing;

	private Polygon2D _polygon2D;
	private CollisionPolygon2D _collisionPolygon2D;

    public override void _Ready()
    {
        _polygon2D = GetChild<Polygon2D>(1);
		_collisionPolygon2D = GetChild<CollisionPolygon2D>(0);
    }

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
	public void SetShape(Vector2[] polygon) {
		_polygon2D.Polygon = polygon;
		_collisionPolygon2D.Polygon = polygon;
	}


	// public void ScaleDown(float currScale) {
	// 	var newScale = new Vector2(1, currScale);
	// 	_polygon2D.ApplyScale(newScale);
	// 	_collisionPolygon2D.ApplyScale(newScale);
	// }

	// public void Slice()
	// {
	// 	var instance = _documentScene.Instantiate<Document>();
		
	// 	GetParent().AddChild(instance);

	// 	ScaleDown(0.5f);

	// 	instance.ScaleDown(_polygon2D.Scale.Y);
	// }
	public void Slice(Vector2[] sliceLine) 
	{
		var globalPolygon = _polygon2D.Polygon.Select(ToGlobal).ToArray();
		var polygons = Geometry2D.ClipPolygons(globalPolygon, sliceLine);

		GD.Print(polygons);

		var instance = _documentScene.Instantiate<Document>();
		GetParent().AddChild(instance);

		instance.SetShape(polygons[0]);
		SetShape(polygons[1]);
		
	}
}
