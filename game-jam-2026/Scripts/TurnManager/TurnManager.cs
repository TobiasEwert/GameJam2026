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

	public enum TurnState
	{
		StartGame, // any ui or setup needed at the start of the game
		TurnStart, // the beginning of a player's turn, switch the ui for the new player, any other visual changes
		PlayerAction, // choose an action, end turn, spin, invest, or use an ability card
		ActionWaiting, // waiting for the action to complete, waiting for the wheel to finish spinning, apply the results, return to ActionWaiting and wait for them to continue or end the turn
		TurnEnd, // once the player ends their turn or they bust then transition to the next player, go to turn start
		GameOver
	}

	public enum WedgeType
	{
		Bust,
		BreakEven,
		Lose,
		Double,
		Triple,
		Half,
		// New wedge types are added here
	}

	[Export] public GameConfig gameConfig { get; set; }
	// Set by the gameConfig settings
	public int maxRounds { get; set; }
	public int startingSpinCost { get; set; }

	// Turn manager variables
	public TurnState currentState { get; set; } = TurnState.StartGame;
	public PlayerData currentPlayer { get; set; }
	public PlayerData[] players { get; set; }

	public int currentPlayerIndex { get; set; }
	public int currentRound { get; set; }
	public int spinsThisTurn { get; set; }

	public override void _Ready()
	{
		base._Ready();
		maxRounds = gameConfig.maxRounds;
		startingSpinCost = gameConfig.baseSpinCost;
	}
	
	// Initial setup for the game, starts once the start game button is triggered, can also have the UI transition end call StartTurn
	public void StartGame()
	{
		currentState = TurnState.StartGame;
		currentPlayerIndex = 0;
		currentRound = 1;
		players = new [] {new PlayerData {playerName = "Player 1"}, new PlayerData {playerName = "Player 2"}}; // hardset the names
		EmitSignal(SignalName.GameStarted); // might be removed as it may not be necessary
		EmitSignal(SignalName.RoundChanged, 1);
		StartTurn();
	}

	public void StartTurn()
	{
		currentState = TurnState.TurnStart;
		currentPlayer = players[currentPlayerIndex];
		currentPlayer.roundEarnings = 0; // reset the round earnings at the start of the turn, pot
		spinsThisTurn = 0;
		EmitSignal(SignalName.TurnStarted, currentPlayerIndex);
		currentState = TurnState.PlayerAction;
		// Do Not change state here, change it on the Action Logic, sit on PlayerAction until an action is taken
	}

	// Will be called externally, if true, then it spins, if false, it does not.
	public bool TryPaySpin()
	{
		if(currentState != TurnState.PlayerAction)
		{
			return false; // if state is not waiting for player action, do not try to spin
		}

		int spinCost = startingSpinCost * spinsThisTurn;
		if(currentPlayer.currency < spinCost)
		{
			return false; // not enough money
		}

		currentPlayer.currency -= spinCost;
		spinsThisTurn++;
		currentState = TurnState.ActionWaiting;

		EmitSignal(SignalName.MoneyChanged, currentPlayerIndex, currentPlayer.currency);

		return true;
	}
	

}
