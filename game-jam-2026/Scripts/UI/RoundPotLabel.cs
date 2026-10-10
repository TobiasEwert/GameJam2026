using Godot;
using System;

public partial class RoundPotLabel : Label
{
	public TurnManager turnManager;
	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		turnManager.PotChanged += OnRoundPotChanged;
	}

	private void OnRoundPotChanged(int playerIndex, int potAmount)
	{
		this.Text = $"Round Pot:${potAmount}";
	}
}
