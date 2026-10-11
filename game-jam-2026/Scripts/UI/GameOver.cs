using Godot;
using System;

public partial class GameOver : Control
{
public TurnManager turnManager;
[Export] public Label winningPlayer;
[Export] public Label p1Money;
[Export] public Label p2Money;

public override void _Ready()
{
    turnManager = TurnManager.Instance;
	if(turnManager.players[0].currency >= turnManager.players[1].currency)
	{
		winningPlayer.Text = $"{turnManager.players[0].playerName} Wins!";
	} else {
		winningPlayer.Text = $"{turnManager.players[1].playerName} Wins!";
	}
	p1Money.Text = $"p1: ${turnManager.players[0].currency}";
	p2Money.Text = $"p2: ${turnManager.players[0].currency}";
}

}
