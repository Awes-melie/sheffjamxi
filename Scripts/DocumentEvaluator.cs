using Godot;
using System;

public partial class DocumentEvaluator : Node
{
    public static DocumentResponse EvaluateDocument(Document document)
    {
        return new DocumentResponse(true, "");
    }
}
