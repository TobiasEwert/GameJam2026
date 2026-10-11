using Godot;
using System;

public partial class StartMenu : Control
{
	public override void _Ready()
	{
		base._Ready();
	}

	public void StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("2DScenes/Levels/MainScene.tscn");
	}
}
