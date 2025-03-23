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
		this.Text = "";
	}

	private string _text;

	public void ShowDialog(string text)
	{
		//var textElement = GetChild<RichTextLabel>(0);
		var animationElement = GetChild<AnimationPlayer>(0);

		this.Text = text;
		animationElement.Queue("play_text");
	}
}
