using Godot;

public partial class CardObject2D : GridContainer
{
  [Signal] public delegate void CardSelectedEventHandler(CardData selectedData);
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
    //_cardNameLabel.Text = AssignedCardData.CardName;

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

      int index = i;                       
      btn.Pressed += () => _on_button_pressed(index);
      _cardInventory.AddChild(btn);
     }
}
  public void _on_button_pressed(int index)
  {
      EmitSignal(SignalName.CardSelected, activePlayer.abilityCards[index]);
      audioPlayer.Stream = cardUsed ;
			audioPlayer.Play();
      activePlayer.abilityCards.RemoveAt(index);
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
      audioPlayer.Stream = cardGained;
			audioPlayer.Play();
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
		ShowPopup("Opponent has no cards to steal.");
		return;
	}
	if(activePlayer.abilityCards.Count >= 4)
	{
		ShowPopup("Your card collection is full.");
		return;
	}
	
	RandomNumberGenerator rng = new RandomNumberGenerator();
	rng.Randomize();
	
	int randomIndex =
		rng.RandiRange(0, opponent.abilityCards.Count - 1);
		
	CardData stolenCard = opponent.abilityCards[randomIndex];
	
	opponent.abilityCards.RemoveAt(randomIndex);
	activePlayer.abilityCards.Add(stolenCard);
	ShowPopup(
		$"{activePlayer.playerName} stole a random card!\n" +
		$"Card {stolenCard.CardName}"
	);
	RebuildCardInventoryDisplay();
  }

  private PlayerData GetOpponent()
  {
	if(_Turn.players == null || _Turn.players.Length < 2)
		return null;
	
	int opponentIndex = (_Turn.currentPlayerIndex + 1) % _Turn.players.Length;
	return _Turn.players[opponentIndex];
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
	int randomIndex =
		rng.RandiRange(0, opponent.abilityCards.Count - 1);
	CardData card = opponent.abilityCards[randomIndex];
	ShowPopup(
		$"Opponent card:\n\n" +
		$"{card.CardName}\n" +
		$"{card.Description}"
	);
  }

  private void ActivateOpponentBet()
  {
	PlayerData opponent = GetOpponent();
	if(opponent == null)
		return;

	ShowPopup($"{activePlayer.playerName} used Opponent Bet!");
  }

  private void ShowPopup(string message)
  {
	_Turn.EmitSignal(TurnManager.SignalName.PopupMessage, message);
  }
}
