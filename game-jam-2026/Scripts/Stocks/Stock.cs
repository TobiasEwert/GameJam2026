using Godot;
using System;

public partial class Stock : Node
{
  // Add Signals for the ui
  [Export] public float CurrentValue = 100f;
  private Random _random = new Random();

  public TurnManager turnManager;

    public override void _Ready()
    {
        base._Ready();
        turnManager = TurnManager.Instance;
    }


  public void AdvanceMarketRound()
  {
    float marketSwing = (float)(_random.NextDouble() * 0.60 - 0.25);
    CurrentValue += CurrentValue * marketSwing;
    if(CurrentValue < 1.0f)
    {
      CurrentValue = 1.0f;
    }
  }

  public void BuyStock(PlayerData player, int cashToInvest)
  {
    if(turnManager.TryPay(cashToInvest))
    {
      player.stockShares = cashToInvest / CurrentValue;
    }
  }

  public void SellStock(PlayerData player)
  {
    if(player.stockShares > 0)
    {
      int payoutValue = Mathf.RoundToInt(player.stockShares * CurrentValue);

      player.currency += payoutValue;
      player.stockShares = 0;
    }
  }
}
