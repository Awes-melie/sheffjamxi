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
            if(rule.Item1.Invoke(document) == false)
            {
                return new DocumentResponse(rule.Item3 ? ValidationResult.FAIL : ValidationResult.MISTAKE, rule.Item2);
            }
        }
        return new DocumentResponse(ValidationResult.WIN, "");
    }
    
    // Check, Error Message, Is critical error
    private static List<Tuple<Func<Document,bool>, string, bool>> _rules = 
    [   

        // Rule 1

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("7c", FilledType.PEN), "Please check tick box 7c of section 3s", false),


        // Rule 2

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("email",FilledType.UNFILLED), "Do not fill in the asterix'd fields, Please! Start Over.", true),
        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("address",FilledType.UNFILLED), "Do not fill in the asterix'd fields, Please! Start Over.", true),

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("rectangle",FilledType.PEN), "You are missing entries in the starred fields", false),
        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("favouriteColour",FilledType.PEN), "You are missing entries in the starred fields", false),
        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("symptoms",FilledType.PEN), "You are missing entries in the starred fields", false),

        // Rule 3

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckFieldHasDrawing("symptoms", true), "Your symptoms seem too vague, draw them in the box provided.", false),

        // Rule 4

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldColour("19.2", false), "This section is too large, politely trim section 19.2", false),

        // Rule 5

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldColour("7b", false), "This form has outdated information, kindly remove section 7b", false),

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldColour("returnAddress", true), "This form is NOW missing the return address, You will need to start over.", true),

        // Rule 6
        // TODO: Implement Oranges
        //new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("oranges",FilledType.PEN), "You are missing entries in the starred fields", false),


        // Rule 7

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckFieldHasDrawing("dottedLine", true), "We require a signature. Sign the dotted line, if you please", false),
        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckFieldHasDrawing("dashedLine", false), "You have signed on the dashed line, when I clearly stated the dotted line. Start Over.", true),

        // Rule 8

        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("compliments",FilledType.PEN), "We are collecting customer feedback, please fill in the survey.", false),
        new Tuple<Func<Document,bool>,string,bool>(doc => doc.CheckHasFieldInState("complaints",FilledType.UNFILLED), "We aren’t currently accepting complaints, please start again", true),

        //new Tuple<Func<Document,bool>, string, bool>(doc => doc.SizeDocument() < 500000 , "Document too small!", true),
        //new Tuple<Func<Document,bool>, string, bool>(doc => doc.SizeDocument() > 1900000, "Document too big!", false),
        //new Tuple<Func<Document,bool>, string, bool>(doc => doc.CheckHasFieldInState("test", FilledType.PEN), "Need test field", false),
    ];

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
