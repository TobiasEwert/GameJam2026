using Godot;
using System;

public partial class StartMenu : Control
{
	[Export] public Button StartButton;
	[Export] public PackedScene MainScene; 
	public override void _Ready()
	{
		StartButton.Pressed += StartButtonPressed;
	}

public void StartButtonPressed()
{
    if (MainScene == null)
    {
        GD.PushError("StartMenu: MainScene not assigned in the inspector");
        return;
    }

    var err = GetTree().ChangeSceneToPacked(MainScene);
    if (err != Error.Ok)
        GD.PushError($"StartMenu: couldn't change scene: {err}");
}
}
