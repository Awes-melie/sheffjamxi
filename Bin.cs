using Godot;
using System;
using System.Collections.Generic;

public partial class Bin : Area2D
{

    private bool _grabbing;
    private List<Document> _documents = new List<Document>();

    public void _on_body_entered(Node2D body)
    {
        if(body is not Document document) return;
        _documents.Add(document);
    }
    
    public void _on_body_exited(Node2D body)
    {
       if(body is not Document document) return;
       if (!_documents.Contains(document)) return;
        _documents.Remove(document);
    }
    
    public override void _UnhandledInput(InputEvent eventArgs)
	{
		if (eventArgs is InputEventMouseButton mouseEvents)
		{
			_grabbing = mouseEvents.Pressed && mouseEvents.ButtonIndex == MouseButton.Left;

            if (!_grabbing) {
                foreach (Document document in _documents) {
                    document.QueueFree();
                }
            }
        }
    }
}
