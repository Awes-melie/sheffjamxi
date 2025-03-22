using Godot;
using System;
using System.Collections.Generic;

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
        var currentState = _stateIndex[index].FilledType;
        
        if (currentState == FilledType.PEN) return; // Can't overwrite pen

        if (currentState == FilledType.UNFILLED)
        {
            var newState = clickEvent.ToolType;
            if(newState == ToolType.PEN)
            {
                _stateIndex[index].FilledType = FilledType.PEN;
            } else if (newState == ToolType.PENCIL)
            {
                _stateIndex[index].FilledType = FilledType.PENCIL;
            }
        }
        if (currentState == FilledType.PENCIL)
        {
            var newState = clickEvent.ToolType;
            if(newState == ToolType.PEN)
            {
                _stateIndex[index].FilledType = FilledType.PEN;
            } else if (newState == ToolType.RUBBER)
            {
                _stateIndex[index].FilledType = FilledType.UNFILLED;
            }
        }
        if (currentState != _stateIndex[index].FilledType)
        {
            UpdateTexture(index + Enum.GetName(_stateIndex[index].FilledType));
        }
    }

    private void UpdateTexture(string index)
    {
        if (!_appliedTextures.Remove(index))
        {
            _appliedTextures.Add(index);
        }
    }

    private Texture _clickRegions = ResourceLoader.Load<Texture>("res://ClickAreaIndex.png");

    private Dictionary<Color,string> _colourIndex = new Dictionary<Color, string>
    {
        {new Color(1,0,0), "test"}
    };

    private Dictionary<string,Action<DocumentClickEvent>> _actionIndex;

    private Dictionary<string,FieldState> _stateIndex = new Dictionary<string, FieldState>()
    {
        {"test", new FieldState() {FilledType = FilledType.UNFILLED}}
    };

    private Dictionary<string,Texture> _textureIndex = new Dictionary<string, Texture>()
    {
        {$"test{nameof(FilledType.PEN)}", ResourceLoader.Load<Texture>("res://TextOverlaySprites/TestOverlay.png")}
    };

    private List<string> _appliedTextures = [];
}
