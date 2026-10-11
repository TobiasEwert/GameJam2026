using Godot;
using System;

public partial class RoundPotLabel : Label
{
	public TurnManager turnManager;
	Color positivePot = new Color("#3FBF5F");
    Color zeroPot = new Color("#e8b84a");
    Color negativePot = new Color("#FF4D4D");
	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		turnManager.PotChanged += OnRoundPotChanged;
		turnManager.TurnStarted += OnTurnStarted;
	}

	private void OnRoundPotChanged(int playerIndex, int potAmount)
	{
		this.Text = $"Pot: ${potAmount}";
		if(potAmount > 0)
		{
			this.LabelSettings.FontColor = positivePot;
		}
		else if(potAmount == 0)
		{
			this.LabelSettings.FontColor = zeroPot;
		}
		else
		{
			this.LabelSettings.FontColor = negativePot;
		}
	}
	private void OnTurnStarted(int playerIndex)
	{
		OnRoundPotChanged(playerIndex, turnManager.currentPlayer.roundEarnings);
	}
}
