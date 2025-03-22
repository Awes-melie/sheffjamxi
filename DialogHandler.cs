using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class DialogHandler : RichTextLabel
{
    [Export]
    public int CharSpeed { get; set; }
    public static DialogHandler Instance;

    public DialogHandler()
    {
        Instance ??= this;
    }

    private string _text;

    public void ShowDialog(string text)
    {
        Text = text;
    }
}
