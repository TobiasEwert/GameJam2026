using Godot;
using System;

public partial class Stock : Node
{
  // Add Signals for the ui
  public float CurrentValue = 100f;
  private Random _random = new Random();

  public TurnManager turnManager;
  public static Stock Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        turnManager = TurnManager.Instance;
        turnManager.RoundChanged += OnRoundChanged;
    }
    private void OnRoundChanged(int round)
    {
        AdvanceMarketRound();
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
      player.currency += payoutValue;
    }
  }
}
