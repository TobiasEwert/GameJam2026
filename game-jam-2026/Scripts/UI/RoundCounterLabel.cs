using Godot;
using System;

public partial class RoundCounterLabel : Label
{
	public TurnManager turnManager;
	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		turnManager.RoundChanged += OnRoundChanged;
		turnManager.TurnStarted += OnTurnStarted;
		turnManager.GameEnded += OnRoundChanged;
	}

	private void OnRoundChanged(int newRound)
	{
		this.Text = $"{newRound}";
		if(newRound == 1)
		{
			Color finalRound = new Color("#C0392B");
			this.LabelSettings.FontColor = finalRound;
		}
	}
	private void OnTurnStarted(int playerIndex)
	{
		OnRoundChanged(turnManager.currentRound);
	}
}
