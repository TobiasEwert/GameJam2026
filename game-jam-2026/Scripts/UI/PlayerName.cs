using Godot;
using System;

public partial class PlayerName : Label
{
	public TurnManager turnManager;
	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		turnManager.TurnStarted += OnTurnStarted;
	}

	private void OnTurnStarted(int playerIndex)
	{
		this.Text = turnManager.currentPlayer.playerName;
	}
}
