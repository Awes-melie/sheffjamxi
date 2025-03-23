using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DocumentEvaluator
{
    public static float ValidFieldColourProportion { get; set; } = 0.95f;

    private static Dictionary<Color,List<Vector2I>> _pixelMap = InitialisePixelArray();
    private static Dictionary<Vector2I,Color> _colourMap;

    private static Dictionary<Color, List<Vector2I>> InitialisePixelArray()
    {
        _colourMap = new Dictionary<Vector2I,Color>();

        var texture = ResourceLoader.Load<Texture2D>("res://ClickAreaIndex.png").GetImage();
        
        var dictionary = new Dictionary<Color, List<Vector2I>>();

        for (int x = 0; x < texture.GetWidth(); x++)
        {
            for (int y = 0; y < texture.GetHeight(); y++)
            {
                var colour = texture.GetPixel(x,y);
                var point = new Vector2I(x,y);
                if(!dictionary.ContainsKey(colour))
                {
                    dictionary.TryAdd(colour, [point]);
                    _colourMap.TryAdd(point, colour);
                } else {
                    dictionary[colour].Add(point);
                    _colourMap.TryAdd(point, colour);
                }
            }   
        }

        return dictionary;
    }

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

    public static bool CheckHasFieldColour(this Document document, string field, bool present)
    {
        var colour = DocumentUvMapper.ColourIndex.FirstOrDefault(x => x.Item2 == field).Item1;

        var sum = 0;
        foreach (var pixel in _pixelMap[colour])
        {
            if (document.IsPointInDocument(pixel))
            {
                sum ++;
            }
        }
        
        var proportion = (float)sum / _pixelMap[colour].Count;
        return present ? proportion > ValidFieldColourProportion : proportion < 1 - ValidFieldColourProportion ;
    }

        public static bool CheckFieldHasDrawing(this Document document, string field, bool present)
    {
        var colour = DocumentUvMapper.ColourIndex.FirstOrDefault(x => x.Item2 == field).Item1;
        
        var lines = document.Polygon2D.GetChildren().Cast<Line2D>();
        var points = lines.SelectMany(x => x.Points).Select(y => (Vector2I)y.Floor());

        foreach (var point in points)
        {
            if(_colourMap[CoordinateUtilities.WorldToTexture(point)] == colour)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsPointInDocument(this Document document, Vector2I point)
    {
        var uv = document.Polygon2D.UV;
        var mappedUV = uv.Select(x => (Vector2)CoordinateUtilities.WorldToTexture(x)).ToArray();
        return Geometry2D.IsPointInPolygon(point, mappedUV);
    }
}
