using Godot;
using System;
using System.Collections.Generic;

public partial class Stock : Node
{
  // Add Signals for the ui
  [Signal] public delegate void PriceChangedEventHandler(float newValue, float oldValue);
  [Signal] public delegate void SharesChangedEventHandler(int playerIndex, float shares);

  public float CurrentValue {get; set;} = 100f;
  public List<float> History { get; } = new(); 
  private Random _random = new Random();

  public TurnManager turnManager;
  public static Stock Instance { get; private set; }
  public float ValueOf(PlayerData player) => player.stockShares * CurrentValue; // Player value from stocks
    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        turnManager = TurnManager.Instance;
        turnManager.RoundChanged += OnRoundChanged;
        History.Add(CurrentValue);
    }
    private void OnRoundChanged(int round)
    {
        AdvanceMarketRound();
    }

  public void AdvanceMarketRound()
  {
        float old = CurrentValue;
        float swing = (float)(_random.NextDouble() * 0.60 - 0.30); 
        CurrentValue = Mathf.Max(1f, CurrentValue * (1f + swing));
        History.Add(CurrentValue);
        EmitSignal(SignalName.PriceChanged, CurrentValue, old);
  }

  public bool BuyStock(PlayerData player, int cashToInvest)
  {
      if (cashToInvest <= 0 || !turnManager.TryPay(cashToInvest))
      {
          return false;
      }
      turnManager.currentState = TurnManager.TurnState.PlayerAction;
      player.stockShares += cashToInvest / CurrentValue;
      EmitSignal(SignalName.SharesChanged, turnManager.currentPlayerIndex, player.stockShares);
      return true;
  }

  public bool SellAllStocks(PlayerData player)
    {
        PlayerData currentPlayer = turnManager.currentPlayer;
        if (currentPlayer.stockShares <= 0f) 
        {
            return false;
        }
        if (turnManager.currentState != TurnManager.TurnState.PlayerAction)
        {
            return false;
        }

        int payout = Mathf.RoundToInt(ValueOf(currentPlayer));
        currentPlayer.currency += payout;
        turnManager.EmitSignal(TurnManager.SignalName.MoneyChanged, turnManager.currentPlayerIndex, currentPlayer.currency);
        currentPlayer.stockShares = 0f;
        EmitSignal(SignalName.SharesChanged, turnManager.currentPlayerIndex, currentPlayer.stockShares);
        return true;
  }
}
