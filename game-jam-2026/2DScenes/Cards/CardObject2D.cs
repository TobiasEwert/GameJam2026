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
    foreach (Node child in _cardInventory.GetChildren())
    {
      child.QueueFree();
    }

     //Test it to see if it pulls the card icon data successfully
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
        break;
      case 1:
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
}
