using Godot;
using System;

public partial class Stock : Node
{
  [Export] public float CurrentValue = 100.f;
  private Random _random = new Random();

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
    if(player.CurrentMoney >= cashToInvest)
    {
      player.CurrentMoney -= cashToInvest;
      player.InvestedMoney = cashToInvest;
      player.PurchasePrice = CurrentValue;
    }
  }

  public void SellStock(PlayerData player)
  {
    if(player.InvestedMoney >= 0)
    {
      float sharesOwned = player.InvestedMoney / player.PurchasePrice;
      int payoutValue = Mathf.RoundToInt(sharesOwned * CurrentValue);

      player.CurrentMoney += payoutValue;
      player.InvestedMoney = 0;
      player.PurchasePrice = 0.0f;
    }
  }
}
