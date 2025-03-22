using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
	private Polygon2D _shadowPolygon2D;
	private CollisionPolygon2D _collisionPolygon2D;

	private PinJoint2D _pin;

    public override void _Ready()
    {
        _polygon2D = GetChild<Polygon2D>(2);
		_shadowPolygon2D = GetChild<Polygon2D>(1);
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
			_grabbing = mouseEvents.Pressed && mouseEvents.ButtonIndex == MouseButton.Left;
			
			if (!_mouseOver) return;

			if (!_grabbing) return;

			var mousePos = GetGlobalMousePosition();
			_mouseGrabbedPosition = ToLocal(mousePos);

			if (_pin != null) return;

			_pin = new PinJoint2D();
			_pin.Position = _mouseGrabbedPosition;
			_pin.NodeB = GetPath();
			_pin.NodeA = GetParent().GetChild<StaticBody2D>(0).GetPath();
			AddChild(_pin);
		} 
		else if (eventArgs is InputEventMouseMotion)
		{

			if(_grabbing) return;

			if (_pin == null) return;
			_pin.QueueFree();
			_pin = null;
		}
	}
	public void SetShape(Vector2[] polygon) {
		_polygon2D.Polygon = polygon;
		_shadowPolygon2D.Polygon = polygon;
		_polygon2D.UV = polygon;
		_collisionPolygon2D.Polygon = polygon;
	}

	public void Slice(Vector2[] sliceLine) 
	{

		var globalPolygon = _polygon2D.Polygon.Select(ToGlobal).ToArray();
		var polygons = Geometry2D.ClipPolygons(globalPolygon, sliceLine);

		GD.Print(polygons);
		for (int i = 1; i < polygons.Count; i++) {
			var instance = _documentScene.Instantiate<Document>();
			GetParent().AddChild(instance);

			instance.Position = Position;
			instance.Rotation = Rotation;

			instance.SetShape(polygons[i].Select(ToLocal).ToArray());
		}

		SetShape(polygons[0].Select(ToLocal).ToArray());
	}

    public void ApplyTextures(IEnumerable<Image> textures)
    {
		var combinedTexture = textures.Aggregate((imgA, imgB) =>
		{
			var temp = imgA.GetRegion(imgA.GetUsedRect());
			temp.BlendRect(imgB, new Rect2I(0,0,imgB.GetWidth(),imgB.GetHeight()), Vector2I.Zero);
			return temp;
		});

		var baseImage = GetChild<Polygon2D>(-1).Texture.GetImage();
		baseImage.BlendRect(combinedTexture, 
			new Rect2I(0,0,combinedTexture.GetWidth(),combinedTexture.GetHeight()), Vector2I.Zero);
		GetChild<Polygon2D>(-1).Texture = ImageTexture.CreateFromImage(baseImage);
    }

    public Dictionary<string,FieldState> StateIndex { get; } = new Dictionary<string, FieldState>()
    {
        {"test", new FieldState() {FilledType = FilledType.UNFILLED}}
    };

}
