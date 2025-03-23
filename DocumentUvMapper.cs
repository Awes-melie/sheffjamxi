using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DocumentUvMapper
{
    public static DocumentUvMapper Instance { get; } = new DocumentUvMapper();

    public DocumentUvMapper()
    {
        _actionIndex = new Dictionary<string, Action<DocumentClickEvent>>{
            {"rectangle", clickEvent => DefaultClickBehaviour("rectangle",clickEvent)},
            {"favouriteColour", clickEvent => DefaultClickBehaviour("favouriteColour",clickEvent)},
            {"symptoms", clickEvent => DefaultClickBehaviour("symptoms",clickEvent)},
            {"email", clickEvent => DefaultClickBehaviour("email",clickEvent)},
            {"address", clickEvent => DefaultClickBehaviour("address",clickEvent)},
            {"7c", clickEvent => DefaultClickBehaviour("7c",clickEvent)},
            {"compliments", clickEvent => DefaultClickBehaviour("compliments",clickEvent)},
            {"complaints", clickEvent => DefaultClickBehaviour("complaints",clickEvent)},
        };

        DialogHandler.Instance.ShowDialog("Hello, World!");
    }

    public void ClickDocument(DocumentClickEvent clickEvent)
    {
        var colour = _clickRegions.GetPixel(clickEvent.TextureCoordinates.X, clickEvent.TextureCoordinates.Y);
        
        GD.Print($"{clickEvent.TextureCoordinates}");

        if(colour == new Color(1,1,1)) return; // If white (background, ignore)

        var index = ColourIndex.FirstOrDefault(x => x.Item1 == colour);

        if(index == null) return;

        _actionIndex[index.Item2].Invoke(clickEvent); // Do click action
    }

    public void DefaultClickBehaviour(string index, DocumentClickEvent clickEvent)
    {
        var document = clickEvent.Document;
        var stateIndex = document.StateIndex;

        if(!stateIndex.ContainsKey(index)) return;

        var currentState = stateIndex[index].FilledType;

        stateIndex[index].FilledType = FilledType.PEN;

        GD.Print($"{index}");

        if (currentState != stateIndex[index].FilledType)
        {
            stateIndex[index].CurrentTexture = _textureIndex[index];

            document.ApplyTextures();
        }
    }

    private Image _clickRegions = ResourceLoader.Load<Texture2D>("res://ClickAreaIndex.png").GetImage();

    public static List<Tuple<Color,string>> ColourIndex {get;} = new List<Tuple<Color, string>>
    {
        new Tuple<Color, string>(new Color(109, 0, 60, 1),"rectangle"),
        new Tuple<Color, string>(new Color(66, 161, 20, 1),"favouriteColour"),
        new Tuple<Color, string>(new Color(149, 245, 181, 1),"symptoms"),
        new Tuple<Color, string>(new Color(81, 79, 168, 1),"email"),
        new Tuple<Color, string>(new Color(206, 65, 105, 1),"address"),
        new Tuple<Color, string>(new Color(208, 22, 192, 1),"7c"),
        new Tuple<Color, string>(new Color(20, 119, 112, 1),"dashedLine"),
        new Tuple<Color, string>(new Color(222, 29, 219, 1),"dottedLine"),
        new Tuple<Color, string>(new Color(52, 96, 72, 1),"7b"),
        new Tuple<Color, string>(new Color(113, 247, 15, 1),"returnAddress"),
        new Tuple<Color, string>(new Color(78, 68, 32, 1) ,"19.2"),
        new Tuple<Color, string>(new Color(56, 251, 205, 1),"compliments"),
        new Tuple<Color, string>(new Color(120, 235, 215, 1),"complaints"),
    };

    private Dictionary<string,Action<DocumentClickEvent>> _actionIndex;

    private Dictionary<string,Texture2D> _textureIndex = new Dictionary<string, Texture2D>()
    {
        {"rectangle", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/rectangle.png")},
        {"favouriteColour", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/favouriteColour.png")},
        {"symptoms", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/symptoms.png")},
        {"email", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/email.png")},
        {"address", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/address.png")},
        {"7c", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/7c.png")},
        {"compliments", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/compliments.png")},
        {"complaints", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/complaints.png")},
    };
}
