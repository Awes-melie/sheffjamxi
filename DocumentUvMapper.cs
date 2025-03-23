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
            {"alias", clickEvent => { DefaultClickBehaviour("alias",clickEvent); } },
            {"name", clickEvent => { DefaultClickBehaviour("name",clickEvent); } },
            {"john", clickEvent => { DefaultClickBehaviour("john",clickEvent); } },
            {"age", clickEvent => {DefaultClickBehaviour("age",clickEvent);}},
            {"dislikedColour", clickEvent => {DefaultClickBehaviour("dislikedColour",clickEvent);}},
            {"oranges", clickEvent => {DefaultClickBehaviour("oranges",clickEvent);}},
            {"tlc", clickEvent => {DefaultClickBehaviour("tlc",clickEvent);}},
            {"pc", clickEvent => {DefaultClickBehaviour("pc",clickEvent);}},
            {"bbj", clickEvent => {DefaultClickBehaviour("bbj",clickEvent);}},
            {"8c", clickEvent => {DefaultClickBehaviour("8c",clickEvent);}},
            {"9c", clickEvent => {DefaultClickBehaviour("9c",clickEvent);}},
            {"pk", clickEvent => {DefaultClickBehaviour("pk",clickEvent);}},
            {"tmdd", clickEvent => {DefaultClickBehaviour("tmdd",clickEvent);}},
            {"faxNo", clickEvent => {DefaultClickBehaviour("faxNo",clickEvent);}},
            {"landline", clickEvent => {DefaultClickBehaviour("landline",clickEvent);}},
            {"attempt", clickEvent => {DefaultClickBehaviour("attempt",clickEvent);}},
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
        new Tuple<Color,string>(new Color(141/255f,118/255f,178/255f), "name"),
        new Tuple<Color,string>(new Color(163/255f,119/255f,177/255f), "john"),
        new Tuple<Color,string>(new Color(148/255f,119/255f,177/255f), "age"),
        new Tuple<Color,string>(new Color(185/255f,85/255f,108/255f), "dislikedColour"),
        new Tuple<Color,string>(new Color(183/255f,107/255f,16/255f), "oranges"),
        new Tuple<Color,string>(new Color(183/255f,17/255f,16/255f), "tlc"),
        new Tuple<Color,string>(new Color(183/255f,17/255f,20/255f), "bbj"),
        new Tuple<Color,string>(new Color(183/255f,17/255f,50/255f), "9c"),
        new Tuple<Color,string>(new Color(183/255f,17/255f,70/255f), "pk"),
        new Tuple<Color,string>(new Color(183/255f,16/255f,139/255f), "pc"),
        new Tuple<Color,string>(new Color(183/255f,16/255f,100/255f), "8c"),
        new Tuple<Color,string>(new Color(183/255f,15/255f,140/255f), "tmdd"),
        new Tuple<Color,string>(new Color(17/255f,183/255f,151/255f), "faxNo"),
        new Tuple<Color,string>(new Color(17/255f,183/255f,100 /255f), "landline"),
        new Tuple<Color,string>(new Color(61/255f,118/255f,135 /255f), "attempt"),

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
        {"name", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/name.png")},
        {"john", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/john.png")},
        {"age", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/age.png")},
        {"dislikedColour", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/dislikedColour.png")},
        {"oranges", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/oranges.png")},
        {"tlc", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/tlc.png")},
        {"pc", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/pc.png")},
        {"bbj", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/bbj.png")},
        {"8c", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/8c.png")},
        {"9c", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/9c.png")},
        {"pk", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/pk.png")},
        {"tmdd", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/tmdd.png")},
        {"faxNo", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/faxNo.png")},
        {"landline", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/landline.png")},
        {"attempt", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/attempt.png")},
    };
}
