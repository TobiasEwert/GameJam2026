using Godot;
using System;

[GlobalClass]
public partial class SpinCost : Label
{
	private TurnManager turnManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		Text = $"Spin: ${turnManager.nextSpinCost}";
	}

	public void RefreshSpinCost()
	{
		Text = $"Spin: ${turnManager.nextSpinCost}";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
}
