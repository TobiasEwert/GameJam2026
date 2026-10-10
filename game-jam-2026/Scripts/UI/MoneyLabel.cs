using Godot;
using System;

public partial class MoneyLabel : Label
{
	public TurnManager turnManager;
	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		turnManager.MoneyChanged += OnMoneyChanged;
	}

	private void OnMoneyChanged(int playerIndex, int money)
	{
		this.Text = $"Money: ${money}";
	}

}
