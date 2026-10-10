using Godot;

public partial class CardObject2D : Control
{
  [Signal] public delegate void CardSelectedEventHandler(CardData selectedData);
  [Export] public CardData AssignedCardData;

  private Label _cardNameLabel;
  private TextureRect _Cards;
  private GridContainer _cardInventory;
  public TurnManager _Turn;
  public PlayerData activePlayer => _Turn.currentPlayer;

  public override void _Ready()
  {
    _cardNameLabel = GetNode<Label>("Item");
    if(AssignedCardData != null)
    {
      _cardNameLabel.Text = AssignedCardData.CardName;
    }
    _Cards = GetNode<TextureRect>("TextureRect");
    _cardInventory = GetNode<GridContainer>("TextureRect/GridContainer");
    _Turn = TurnManager.Instance;
    
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
      //btn.Pressed += () => Player.UseInventoryItem(index);

      _cardInventory.AddChild(btn);
  }
}
  // Test to make sure the function is compatible with grid container
  public void _on_button_pressed()
  {
    if(AssignedCardData != null)
    {
      EmitSignal(SignalName.CardSelected, Variant.From(AssignedCardData));
      QueueFree();
    }
  }
}
