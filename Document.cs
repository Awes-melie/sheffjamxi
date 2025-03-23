using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

public partial class Document : RigidBody2D
{
	[Export]
	public float MovementScalar{get; set;}
	public Polygon2D Polygon2D { get; private set; }

	private PackedScene _documentScene = ResourceLoader.Load<PackedScene>("res://Document.tscn");

	private Vector2 _mouseGrabbedPosition;
	private bool _mouseOver;
	private bool _grabbing;
	private Line2D _line;
	private Polygon2D _shadowPolygon2D;
	private CollisionPolygon2D _collisionPolygon2D;
    private Image _baseImage;
    private PinJoint2D _pin;

    public override void _Ready()
    {
        Polygon2D = GetChild<Polygon2D>(2);
		Polygon2D.ClipChildren = ClipChildrenMode.AndDraw;
		_shadowPolygon2D = GetChild<Polygon2D>(1);
		_collisionPolygon2D = GetChild<CollisionPolygon2D>(0);

		_baseImage = Polygon2D.Texture.GetImage();
		_baseImage.Convert(Image.Format.Rgba8);
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

			var toolType = Hand.Instance.toolType;

			if (toolType != ToolType.HAND) 
			{
<<<<<<< HEAD
				// var documentClickEvent = new DocumentClickEvent(toolType, (Vector2I)((_mouseGrabbedPosition + new Vector2(128, 192))*2.5f) , this);
				// GD.Print("pre " , _mouseGrabbedPosition + new Vector2(128, 192));
				// GD.Print("post " , (_mouseGrabbedPosition + new Vector2(128, 192))*2.5f);

=======
				var documentClickEvent = new DocumentClickEvent(toolType, CoordinateUtilities.WorldToTexture(_mouseGrabbedPosition) , this);
>>>>>>> 306e8f4 (Colour area detection on evaluator)

				// DocumentUvMapper.Instance.ClickDocument(documentClickEvent);

				

				_line = new Line2D();
				_line.Width = 2;
				Polygon2D.AddChild(_line);

				if (toolType == ToolType.PEN) {_line.DefaultColor = Colors.Black;}
				if (toolType == ToolType.PENCIL) {_line.DefaultColor = Colors.Gray;}
						

				return;
				}
				


			if (_pin != null) return;

			_pin = new PinJoint2D();
			_pin.Position = _mouseGrabbedPosition;
			_pin.NodeB = GetPath();
			_pin.NodeA = Hand.Instance.GetPath();
			AddChild(_pin);
		} 
		else if (eventArgs is InputEventMouseMotion)
		{

			if(_grabbing && _line != null) {
				var mousePos = GetGlobalMousePosition();
				_mouseGrabbedPosition = ToLocal(mousePos);
				_line.Points = _line.Points.Append(_mouseGrabbedPosition).ToArray(); 
				return;
			}

			_line = new Line2D();



			if (_pin == null) return;
			_pin.QueueFree();
			_pin = null;
		}
	}
	public void SetShape(Vector2[] polygon) {
		Polygon2D.Polygon = polygon;
		_shadowPolygon2D.Polygon = polygon;
		Polygon2D.UV = polygon;
		_collisionPolygon2D.Polygon = polygon;
	}

	public void Slice(Vector2[] sliceLine) 
	{

		var globalPolygon = Polygon2D.Polygon.Select(ToGlobal).ToArray();
		var polygons = Geometry2D.ClipPolygons(globalPolygon, sliceLine);

		GD.Print(polygons);
		for (int i = 1; i < polygons.Count; i++) {
			var instance = _documentScene.Instantiate<Document>();
			GetParent().AddChild(instance);

			instance.Position = Position;
			instance.Rotation = Rotation;
			instance.StateIndex = StateIndex.ToDictionary(e => e.Key, e => e.Value.Clone());
			instance.ApplyTextures();

			foreach (Node child in Polygon2D.GetChildren()) {
				var duplicateChild = child.Duplicate();
				instance.Polygon2D.AddChild(duplicateChild);
			}

			instance.SetShape(polygons[i].Select(ToLocal).ToArray());

		}
		SetShape(polygons[0].Select(ToLocal).ToArray());
<<<<<<< HEAD
=======
		
		GD.Print(this.CheckHasFieldColour("test", true));
>>>>>>> 306e8f4 (Colour area detection on evaluator)
	}

    public void ApplyTextures()
    {
		
		var textures = StateIndex
			.Select(x =>x.Value.CurrentTexture?.GetImage())
			.Where(x => x != null);

		if (textures.Count() == 0) return;

		var combinedTexture = textures.Aggregate((imgA, imgB) =>
		{
			var temp = imgA.GetRegion(imgA.GetUsedRect());
			temp.BlendRect(imgB, new Rect2I(0,0,imgB.GetWidth(),imgB.GetHeight()), Vector2I.Zero);
			return temp;
		});

		var newImage = _baseImage.GetRegion(_baseImage.GetUsedRect());
	
		newImage.BlendRect(combinedTexture, 
			new Rect2I(0,0,combinedTexture.GetWidth(),combinedTexture.GetHeight()), Vector2I.Zero);
		Polygon2D.Texture = ImageTexture.CreateFromImage(newImage);
    }

    public Dictionary<string,FieldState> StateIndex { get; set; } = new Dictionary<string, FieldState>()
    {
        {"test", new FieldState() {FilledType = FilledType.UNFILLED}}
    };

}