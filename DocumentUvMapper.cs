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

        var index = _colourIndex.GetValueOrDefault(colour);

        if(index == null) return;

        _actionIndex[index].Invoke(clickEvent); // Do click action
    }

    public void DefaultClickBehaviour(string index, DocumentClickEvent clickEvent)
    {
        if(!clickEvent.Document.StateIndex.ContainsKey(index)) return;

        var currentState = clickEvent.Document.StateIndex[index].FilledType;
        
        if (currentState == FilledType.PEN) return; // Can't overwrite pen

        var toolUsed = clickEvent.ToolType;

        if (currentState == FilledType.UNFILLED)
        {
            if(toolUsed == ToolType.PEN)
            {
                clickEvent.Document.StateIndex[index].FilledType = FilledType.PEN;
            } 
            else if (toolUsed == ToolType.PENCIL)
            {
                clickEvent.Document.StateIndex[index].FilledType = FilledType.PENCIL;
            }
        }
        if (currentState == FilledType.PENCIL)
        {
            if(toolUsed == ToolType.PEN)
            {
                clickEvent.Document.StateIndex[index].FilledType = FilledType.PEN;
            } 
            else if (toolUsed == ToolType.RUBBER)
            {
                clickEvent.Document.StateIndex[index].FilledType = FilledType.UNFILLED;
            }
        }
        if (currentState != clickEvent.Document.StateIndex[index].FilledType)
        {
            UpdateTexture(index + Enum.GetName(clickEvent.Document.StateIndex[index].FilledType), clickEvent.Document);
        }
    }

    private void UpdateTexture(string index, Document document)
    {
        if (!_appliedTextures.Remove(index))
        {
            _appliedTextures.Add(index);
        }

        var textures = _appliedTextures.Select(x => _textureIndex[x].GetImage());

        document.ApplyTextures(textures);
    }

    private Image _clickRegions = ResourceLoader.Load<Texture2D>("res://ClickAreaIndex.png").GetImage();

    private Dictionary<Color,string> _colourIndex = new Dictionary<Color, string>
    {
        {new Color(1,0,0), "test"}
    };

    private Dictionary<string,Action<DocumentClickEvent>> _actionIndex;

    private Dictionary<string,Texture2D> _textureIndex = new Dictionary<string, Texture2D>()
    {
        {$"test{nameof(FilledType.PEN)}", ResourceLoader.Load<Texture2D>("res://TextOverlaySprites/TestOverlay.png")}
    };

    private List<string> _appliedTextures = [];
}
