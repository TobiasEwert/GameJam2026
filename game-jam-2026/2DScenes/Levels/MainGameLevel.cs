using Godot;
using Godot.Collections;

public partial class MainGameLevel : Node2D
{
  [Export] private CardObject2D _cardTemplate;
  private Label _PlayerName;
  
  private PlayerData _activePlayer;
  private TextureRect _Cards;
  private GridContainer _CardInventory;
  public TurnManager _Turn;

  public override void _Ready()
  {
    _PlayerName = GetNode<Label>("PlayerName");
    _Cards = GetNode<TextureRect>("TextureRect");
    _CardInventory = GetNode<GridContainer>("TextureRect/GridContainer");
    _Turn = TurnManager.Instance;
    // _Turn.StartGame();
  }

  public void UpdateUIElements()
  {
    _PlayerName.Text = _activePlayer.PlayerName;
    _CurrentMoney.Text = $"$ {activePlayer.currency}";
    _RoundNumber.Text = $"Rounds Remaining: {_currentRoundCount}";
  }

  private void RebuildCardInventoryDisplay()
  {
    foreach (Node child in _cardInventory.GetChildren())
    {
      child.QueueFree();
    }

    foreach (CardData card in _activePlayer.activeHandCards)
    {
      if(_cardTemplate.Instantiate() is CardObject2D cardView)
      {
        cardView.AssignedCardData = card;
        _CardInventory.AddChild(cardView);
      }
    }
  }
}
