using Godot;
using System;
using System.Linq;
public partial class DocumentEvaluator : Node
{
    public static DocumentResponse EvaluateDocument(Document document)
    {
        return new DocumentResponse(true, "");
    }

    // Thanks pal https://web.archive.org/web/20100405070507/http://valis.cs.uiuc.edu/~sariel/research/CG/compgeom/msg00831.html
    public static float SizeDocument(Document document)
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
}
