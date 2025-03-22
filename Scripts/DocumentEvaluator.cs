using Godot;
using System;
using System.Collections.Generic;

public static class DocumentEvaluator
{
    public static DocumentResponse EvaluateDocument(Document document)
    {
        foreach (var rule in _rules)
        {
            if(rule.Key.Invoke(document))
            {
                return new DocumentResponse(false, rule.Value);
            }
        }
        return new DocumentResponse(true, "");
    }
    
    private static Dictionary<Func<Document,bool>, string> _rules = new Dictionary<Func<Document,bool>, string>
    {
        {doc => doc.SizeDocument() < 100, "Document too small!"},
        {doc => doc.SizeDocument() > 190000, "Document too big!"},
        {doc => doc.CheckHasFieldInState("test", FilledType.PEN), "Need test field"},
    };

    // Thanks pal https://web.archive.org/web/20100405070507/http://valis.cs.uiuc.edu/~sariel/research/CG/compgeom/msg00831.html
    public static float SizeDocument(this Document document)
    {
        var vertices = document.Polygon2D.UV;
        var area = 0f;
        for (int i = 0; i < vertices.Length; i++)
        {
            var j = Mathf.PosMod(i+1, vertices.Length);
            area += vertices[i].X * vertices[j].Y;
            area -= vertices[i].Y * vertices[j].X;
        }
        return Mathf.Abs(area);
    }

    public static bool CheckHasFieldInState(this Document document, string field, FilledType state)
    {
        return document.StateIndex[field].FilledType == state;
    }
}
