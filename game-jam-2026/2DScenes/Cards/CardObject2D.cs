using Godot;

public partial class CardObject2D : GridContainer
{
  [Signal] public delegate void CardSelectedEventHandler(CardData selectedData);
  private Label _cardNameLabel;
  private TextureRect _Cards;
  private GridContainer _cardInventory;
  public TurnManager _Turn;
  public PlayerData activePlayer => _Turn.currentPlayer;

  public override async void _Ready()
  {
    //_cardNameLabel.Text = AssignedCardData.CardName;

    _Turn = TurnManager.Instance;
    _cardInventory = this;
    await ToSignal(GetTree().CreateTimer(4.0f), "timeout");       
    RebuildCardInventoryDisplay();
  }
  private void RebuildCardInventoryDisplay()
  {
    foreach (Node child in _cardInventory.GetChildren())
    {
      child.QueueFree();
    }

     //Test it to see if it pulls the card icon data successfully
     for (int i = 0; i < activePlayer.abilityCards.Length; i++)
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
  // Test to make sure the function is compatible with grid container
  public void _on_button_pressed(int index)
  {
      EmitSignal(SignalName.CardSelected, activePlayer.abilityCards[index]);
      QueueFree();
  }
}
