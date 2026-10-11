using Godot;

public partial class CardObject2D : GridContainer
{
  [Signal] public delegate void CardSelectedEventHandler(CardData selectedData);

  public const int IncreaseWinningsId = 0;
  public const int UseOpponentCardId = 1;
  public const int ViewOpponentCardId = 2;
  public const int OpponentBetId = 3;
  public const int MaxCards = 4;
  
  private Label _cardNameLabel;
  private TextureRect _Cards;
  private GridContainer _cardInventory;
  public TurnManager _Turn;
  public PlayerData activePlayer => _Turn.currentPlayer;
  [Export] public AudioStreamPlayer2D audioPlayer;
  [Export] public AudioStream cardGained;
  [Export] public AudioStream cardUsed;

  public override void _Ready()
  {
    _Turn = TurnManager.Instance;
    _cardInventory = this;
    _Turn.WedgeResolved += OnWedgeResolved;
    _Turn.TurnStarted += OnTurnStarted;
  }
  
  private void OnTurnStarted(int playerIndex)
  {
    RebuildCardInventoryDisplay();
  }
  private void RebuildCardInventoryDisplay()
  {
    if(activePlayer == null || activePlayer.abilityCards == null || _cardInventory == null)
    {
        return;
    }
    
    foreach (Node child in _cardInventory.GetChildren())
    {
      child.QueueFree();
    }

     for (int i = 0; i < activePlayer.abilityCards.Count; i++)
     {
      var cardData = activePlayer.abilityCards[i];
      var btn = new Button();
      btn.Icon = cardData.Icon;
      btn.ExpandIcon = true;
      btn.CustomMinimumSize = new Vector2(64, 64);
      btn.FocusMode = FocusModeEnum.None;
	  btn.TooltipText = $"{cardData.CardName}\n{cardData.Description}";

      int index = i;                       
      btn.Pressed += () => _on_button_pressed(index);
      _cardInventory.AddChild(btn);
     }
}

public void _on_button_pressed(int index)
{
    if(_Turn.currentState != TurnManager.TurnState.PlayerAction)
	{
		return;
	}
	if(index < 0 || index >= activePlayer.abilityCards.Count)
	{
		return;
	}

	CardData card = activePlayer.abilityCards[index];
	activePlayer.abilityCards.RemoveAt(index);

	if(audioPlayer != null && cardUsed != null)
	{
		audioPlayer.Stream = cardUsed;
		audioPlayer.Play();
	}

	EmitSignal(SignalName.CardSelected, card);
	ActivateCardAbility(card.ActionId);
	RebuildCardInventoryDisplay();
}

  public void ActivateCardAbility(int index)
  {
    switch (index)
    {
      case 0:
	    ActivateIncreaseNextSpin();
        break;
      case 1:
	    ActivateRandomOpponentCard();
        break;
	  case 2:
	    ActivateViewOpponentCard();
		break;
	  case 3:
	    ActivateOpponentBet();
		break;
      default:
        GD.Print("Invalid card index");
        break;
    }
  }
  private void OnWedgeResolved(Wedge wedge, int oldPot, int newPot)
  {
    if(wedge.Type == Wedge.WedgeType.AbilityCard)
    {
      if(audioPlayer != null && cardGained != null)
	  {
		audioPlayer.Stream = cardGained;
		audioPlayer.Play();
	  }
	  RebuildCardInventoryDisplay();
    } 
  }
  
  private void ActivateIncreaseNextSpin()
  {
	_Turn.nextSpinWinningsMultiplier = 2;
	ShowPopup(
		$"{activePlayer.playerName} activated Increase Winnings!\n" +
		"The next spin will pay 2x"
	);
  }

  private void ActivateRandomOpponentCard()
  {
	PlayerData opponent = GetOpponent();
	if(opponent == null)
		return;

	if(opponent.abilityCards.Count == 0)
	{
		ShowPopup("Opponent has no cards to use.");
		return;
	}

	RandomNumberGenerator rng = new RandomNumberGenerator();
	rng.Randomize();
	int randomIndex = rng.RandiRange(0, opponent.abilityCards.Count - 1);

	CardData stolenCard = opponent.abilityCards[randomIndex];
	opponent.abilityCards.RemoveAt(randomIndex);
	
	popupPrefix = $"{activePlayer.playerName} took {stolenCard.CardName} from {opponent.playerName}!\n\n";
	ActivateCardAbility(stolenCard.ActionId);
	popupPrefix = "";
  }

  private void ActivateViewOpponentCard()
  {
	PlayerData opponent = GetOpponent();
	if(opponent == null)
		return;

	if(opponent.abilityCards.Count == 0)
	{
		ShowPopup("Opponent has no cards.");
		return;
	}
	RandomNumberGenerator rng = new RandomNumberGenerator();
	rng.Randomize();
	int randomIndex = rng.RandiRange(0, opponent.abilityCards.Count - 1);
	CardData card = opponent.abilityCards[randomIndex];
	ShowPopup(
		$"{opponent.playerName} is holding:\n\n" +
		$"{card.CardName}\n" +
		$"{card.Description}"
	);
  }

  private void ActivateOpponentBet()
  {
	PlayerData opponent = GetOpponent();
	if(opponent == null)
		return;

	int opponentIndex = (_Turn.currentPlayerIndex + 1) % _Turn.players.Length;
	int amount = _Turn.SetForcedBet(opponentIndex);
	if(amount <= 0)
	{
		ShowPopup($"{opponent.playerName} has no money to bet.");
		return;
	}
	ShowPopup(
		$"{activePlayer.playerName} used Opponent Bet!\n" +
		$"{opponent.playerName} must bet ${amount} on their next spin."
	);
  }

  private PlayerData GetOpponent()
  {
	if(_Turn.players == null || _Turn.players.Length < 2)
		return null;

	int opponentIndex = (_Turn.currentPlayerIndex + 1) % _Turn.players.Length;
	return _Turn.players[opponentIndex];
  }

  private string popupPrefix = "";
  private void ShowPopup(string message)
  {
	_Turn.EmitSignal(TurnManager.SignalName.PopupMessage, popupPrefix + message);
	popupPrefix = "";
  }
}
