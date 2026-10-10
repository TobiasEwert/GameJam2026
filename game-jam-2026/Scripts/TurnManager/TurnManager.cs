using Godot;
using System;

public partial class TurnManager : Node
{
	// Delegates
	[Signal] public delegate void GameStartedEventHandler(); // idk if this is needed, but just in case 
	[Signal] public delegate void GameEndedEventHandler(int winnerIndex);
	[Signal] public delegate void TurnStartedEventHandler(int playerIndex);
	[Signal] public delegate void TurnEndedEventHandler(int playerIndex, bool busted);
	[Signal] public delegate void RoundChangedEventHandler(int round); // rounds count down to zero
	[Signal] public delegate void MoneyChangedEventHandler(int playerIndex, int money); // once you end the turn, the pot is added to the player's money
	[Signal] public delegate void PotChangedEventHandler(int playerIndex, int potAmount); // spin by spin change, this is what will be added/subtracted as you spin/play action cards
	[Signal] public delegate void WedgeResolvedEventHandler(Wedge wedge, int oldPot, int newPot);

	public enum TurnState
	{
		StartGame, // any ui or setup needed at the start of the game
		TurnStart, // the beginning of a player's turn, switch the ui for the new player, any other visual changes
		PlayerAction, // choose an action, end turn, spin, invest, or use an ability card
		ActionWaiting, // waiting for the action to complete, waiting for the wheel to finish spinning, apply the results, return to PlayerAction and wait for them to continue or end the turn
		TurnEnd, // once the player ends their turn or they bust then transition to the next player, go to turn start
		GameOver
	}

	[Export] public GameConfig gameConfig { get; set; }
	// Set by the gameConfig settings
	public int maxRounds { get; set; }
	public int startingSpinCost { get; set; }

	// Turn manager variables
	public static TurnManager Instance { get; private set; }
	public Stock stockInstance { get; private set; }
	public TurnState currentState { get; set; } = TurnState.StartGame;
	public PlayerData currentPlayer { get; set; }
	public PlayerData[] players { get; set; }

	public int currentPlayerIndex { get; set; }
	public int currentRound { get; set; }
	public int spinsThisTurn { get; set; }
	public int nextSpinCost => startingSpinCost * spinsThisTurn;	
	public override void _Ready()
	{
		base._Ready();
		Instance = this;
		stockInstance = Stock.Instance;
		maxRounds = gameConfig.maxRounds;
		startingSpinCost = gameConfig.baseSpinCost;
	}
	
	// Initial setup for the game, starts once the start game button is triggered, can also have the UI transition end call StartTurn
	public void StartGame()
	{
		currentState = TurnState.StartGame;
		currentPlayerIndex = 0;
		currentRound = maxRounds;
		players = new [] {new PlayerData {playerName = "Player 1", currency = gameConfig.startingCurrency }, new PlayerData {playerName = "Player 2", currency = gameConfig.startingCurrency }}; // hardset the names
		EmitSignal(SignalName.GameStarted); // might be removed as it may not be necessary
		EmitSignal(SignalName.RoundChanged, currentRound);
		StartTurn();
	}

	public void StartTurn()
	{
		currentState = TurnState.TurnStart;
		currentPlayer = players[currentPlayerIndex];
		GD.Print("Starting turn for: ", currentPlayer.playerName);
		currentPlayer.roundEarnings = 0; // reset the round earnings at the start of the turn, pot
		spinsThisTurn = 0;
		EmitSignal(SignalName.TurnStarted, currentPlayerIndex);
		currentState = TurnState.PlayerAction;
		// Do Not change state here, change it on the Action Logic, sit on PlayerAction until an action is taken
	}

	// Try To Pay
	public bool TryPay(int amount)
	{
		if(currentPlayer.currency >= amount)
		{
			currentPlayer.currency -= amount;
			EmitSignal(SignalName.MoneyChanged, currentPlayerIndex, currentPlayer.currency);
			currentState = TurnState.ActionWaiting;
			return true;
		}
		return false;
	}

	// Will be called externally, if true, then it spins, if false, it does not.
	// If true go to ActionWaiting state
	public bool TryPaySpin()
	{
		if(currentState != TurnState.PlayerAction)
		{
			return false; // if not waiting for player action, do not try to spin
		}

		if(!TryPay(nextSpinCost))
		{
			return false; // not enough money
		}

		return true;
	}

	public void WedgeAction(Wedge wedge)
	{
		int oldPot = currentPlayer.roundEarnings;
		// The enum is in Wedge.cs add whatever types you want
		switch (wedge.Type)
		{
			case Wedge.WedgeType.Bust:
				ApplyBust(); 
				EndTurn();
				return;
			case Wedge.WedgeType.BreakEven:
				currentPlayer.roundEarnings += nextSpinCost;
				break;
			case Wedge.WedgeType.Lose:
				currentPlayer.roundEarnings -= nextSpinCost;
				break;
			case Wedge.WedgeType.Double:
				currentPlayer.roundEarnings += wedge.Amount * 2;
				break;
			case Wedge.WedgeType.Triple:
				currentPlayer.roundEarnings += wedge.Amount * 3;
				break;
			case Wedge.WedgeType.Half:
				currentPlayer.roundEarnings += wedge.Amount / 2;
				break;
			case Wedge.WedgeType.Add:
				currentPlayer.roundEarnings += wedge.Amount;
				break;
			// Add additional wedge types here
		}
		spinsThisTurn++;
		EmitSignal(SignalName.PotChanged, currentPlayerIndex, currentPlayer.roundEarnings);
		EmitSignal(SignalName.WedgeResolved, wedge, oldPot, currentPlayer.roundEarnings);
		currentState = TurnState.PlayerAction;
	}
	
	public void ApplyBust()
	{
		currentPlayer.roundEarnings = 0;
		EmitSignal(SignalName.PotChanged, currentPlayerIndex, currentPlayer.roundEarnings);
		currentState = TurnState.TurnEnd;
		EmitSignal(SignalName.TurnEnded, currentPlayerIndex, true);
	}
	
	// This saves the roundEarnings and applies them to the player's currency
	public void BankEarnings()
	{
		if(currentState != TurnState.PlayerAction)
		{
			return; 
		}
		currentPlayer.currency += currentPlayer.roundEarnings;
		currentPlayer.roundEarnings = 0;
		EmitSignal(SignalName.MoneyChanged, currentPlayerIndex, currentPlayer.currency);
		EmitSignal(SignalName.PotChanged, currentPlayerIndex, currentPlayer.roundEarnings);
		EndTurn();
	}

	public void EndTurn()
	{
		currentState = TurnState.TurnEnd;
		EmitSignal(SignalName.TurnEnded, currentPlayerIndex, false);
		//TESTING
		AdvanceTurn();
	}

	// Advance turn after curtain close/player transition
	public void AdvanceTurn()
	{
		if(currentState != TurnState.TurnEnd)
		{
			return; 
		}
		int oldPlayerIndex = currentPlayerIndex;
		currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
		if(oldPlayerIndex == 1)
		{
			currentRound--;
			if(currentRound <= 0)
			{
				EndGame();
				return;
			}
			EmitSignal(SignalName.RoundChanged, currentRound);
		}
		StartTurn();
	}

	public void EndGame()
	{
		currentState = TurnState.GameOver;
		int winnerIndex;

		if(players[0].currency > players[1].currency)
		{
			winnerIndex = 0;
		}
		else
		{
			winnerIndex = 1;
		}
		EmitSignal(SignalName.GameEnded, winnerIndex);
	}
}
