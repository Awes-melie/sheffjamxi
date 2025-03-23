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
            {"test", clickEvent => DefaultClickBehaviour("test", clickEvent)}
        };
    }

    public void ClickDocument(DocumentClickEvent clickEvent)
    {
        var colour = _clickRegions.GetPixel(clickEvent.TextureCoordinates.X, clickEvent.TextureCoordinates.Y);

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
        
        if (currentState == FilledType.PEN) return; // Can't overwrite pen

        var toolUsed = clickEvent.ToolType;

        if (currentState == FilledType.UNFILLED)
        {
            if(toolUsed == ToolType.PEN)
            {
                stateIndex[index].FilledType = FilledType.PEN;
            } 
            else if (toolUsed == ToolType.PENCIL)
            {
                stateIndex[index].FilledType = FilledType.PENCIL;
            }
        }
        if (currentState == FilledType.PENCIL)
        {
            if(toolUsed == ToolType.PEN)
            {
                stateIndex[index].FilledType = FilledType.PEN;
            } 
            else if (toolUsed == ToolType.RUBBER)
            {
                stateIndex[index].FilledType = FilledType.UNFILLED;
            }
        }
        if (currentState != stateIndex[index].FilledType)
        {
            stateIndex[index].CurrentTexture = _textureIndex[index + Enum.GetName(stateIndex[index].FilledType)];

            document.ApplyTextures();
        }
    }

    private Image _clickRegions = ResourceLoader.Load<Texture2D>("res://ClickAreaIndex.png").GetImage();

    public static List<Tuple<Color,string>> ColourIndex {get;} = new List<Tuple<Color, string>>
    {
        new Tuple<Color, string>(new Color(1,0,0), "test")
    };

    private Dictionary<string,Action<DocumentClickEvent>> _actionIndex;

    private Dictionary<string,Texture2D> _textureIndex = new Dictionary<string, Texture2D>()
    {
        {$"test{nameof(FilledType.PEN)}", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/TestOverlay.png")}
    };
}
