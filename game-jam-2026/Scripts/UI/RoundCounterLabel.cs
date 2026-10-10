using Godot;
using System;

public partial class RoundCounterLabel : Label
{
	public TurnManager turnManager;
	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		turnManager.RoundChanged += OnRoundChanged;
	}

	private void OnRoundChanged(int newRound)
	{
		this.Text = $"Round: {newRound}";
	}
}
