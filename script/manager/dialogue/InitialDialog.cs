using Godot;
using System;

public partial class InitialDialog : Node
{
	[Export] DialogueData dialogueData;
	public override void _Ready() => CallDeferred(nameof(AddConversionWithDelay));
	void AddConversionWithDelay() => DialogueSystem.Instance.AddConversion(dialogueData);
}
