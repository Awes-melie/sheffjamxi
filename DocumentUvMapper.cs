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
        // Get UV Position
        // Get colour 
        var colour = new Color(0,0,0);
        var index = _colourIndex[colour];

        _actionIndex[index].Invoke(clickEvent); // Do click action
    }

    public void DefaultClickBehaviour(string index, DocumentClickEvent clickEvent)
    {
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

        var textures = _appliedTextures.Select(x => _textureIndex[x]);

        document.ApplyTextures(textures);
    }

    private Texture _clickRegions = ResourceLoader.Load<Texture>("res://ClickAreaIndex.png");

    private Dictionary<Color,string> _colourIndex = new Dictionary<Color, string>
    {
        {new Color(1,0,0), "test"}
    };

    private Dictionary<string,Action<DocumentClickEvent>> _actionIndex;

    private Dictionary<string,Image> _textureIndex = new Dictionary<string, Image>()
    {
        {$"test{nameof(FilledType.PEN)}", ResourceLoader.Load<Image>("res://TextOverlaySprites/TestOverlay.png")}
    };

    private List<string> _appliedTextures = [];
}
