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
            {"alias", clickEvent => { 
                GD.Print("OnClickalias");
                DefaultClickBehaviour("alias",clickEvent); } },
        };
    }

    public void ClickDocument(DocumentClickEvent clickEvent)
    {
        var colour = _clickRegions.GetPixel(clickEvent.TextureCoordinates.X, clickEvent.TextureCoordinates.Y);

        if(colour == new Color(1,1,1)) return; // If white (background, ignore)

        var index = ColourIndex.FirstOrDefault(x => x.Item1 == colour);

        if(index == null) return;

        _actionIndex.GetValueOrDefault(index.Item2)?.Invoke(clickEvent); // Do click action
    }

    public void DefaultClickBehaviour(string index, DocumentClickEvent clickEvent)
    {
        var document = clickEvent.Document;
        var stateIndex = document.StateIndex;

        if(!stateIndex.ContainsKey(index)) return;

        var currentState = stateIndex[index].FilledType;

        stateIndex[index].FilledType = FilledType.PEN;

        if (currentState != stateIndex[index].FilledType)
        {
            stateIndex[index].CurrentTexture = _textureIndex[index];

            document.ApplyTextures();
        }
    }

    private Image _clickRegions = ResourceLoader.Load<Texture2D>("res://ClickAreaIndex.png").GetImage();

    public static List<Tuple<Color,string>> ColourIndex {get;} = new List<Tuple<Color, string>>
    {
        new Tuple<Color, string>(new Color(109/255f, 0/255f, 60/255f, 1),"rectangle"),
        new Tuple<Color, string>(new Color(66/255f, 161/255f, 20/255f, 1),"favouriteColour"),
        new Tuple<Color, string>(new Color(149/255f, 245/255f, 181/255f, 1),"symptoms"),
        new Tuple<Color, string>(new Color(81/255f, 79/255f, 168/255f, 1),"email"),
        new Tuple<Color, string>(new Color(206/255f, 65/255f, 105/255f, 1),"address"),
        new Tuple<Color, string>(new Color(208/255f, 22/255f, 192/255f, 1),"7c"),
        new Tuple<Color, string>(new Color(20/255f, 119/255f, 112/255f, 1),"dashedLine"),
        new Tuple<Color, string>(new Color(222/255f, 29/255f, 219/255f, 1),"dottedLine"),
        new Tuple<Color, string>(new Color(52/255f, 96/255f, 72/255f, 1),"7b"),
        new Tuple<Color, string>(new Color(113/255f, 247/255f, 15/255f, 1),"returnAddress"),
        new Tuple<Color, string>(new Color(78/255f, 68/255f, 32/255f, 1) ,"19.2"),
        new Tuple<Color, string>(new Color(56/255f, 251/255f, 205/255f, 1),"compliments"),
        new Tuple<Color, string>(new Color(120/255f, 235/255f, 215/255f, 1),"complaints"),
        new Tuple<Color, string>(new Color(124/255f, 85/255f, 185/255f, 1),"alias"),
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
        {"alias", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/alias.png")},
    };
}
