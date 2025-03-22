using Godot;

public record DocumentClickEvent(ToolType ToolType, Vector2I TextureCoordinates, Document Document);