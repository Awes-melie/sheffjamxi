using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Guillotine : Area2D
{
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


    public void _on_pressed() {
                var globalPolygon = GetChild<Polygon2D>(1).Polygon.Select(ToGlobal).ToArray();
                foreach (Document document in _documents) {
                    document.Slice(globalPolygon);
                }
            }
}
