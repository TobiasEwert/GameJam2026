using Godot;
using Godot.Collections;

public partial class MainGameLevel : Node2D
{
  [Export] private CardObject2D _cardTemplate;
  private Label _PlayerName;

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

  // private void RebuildCardInventoryDisplay()
  // {

  // }
}
