using Godot;
using Godot.Collections;

public partial class MainGameLevel : Node2D
{
  [Export] private CardObject2D _cardTemplate;
  private Label _PlayerName;
  private PlayerData _activePlayer;

  private Label _currentMoney;
  private Label _RoundMoney;
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

    // Test it to see if it pulls the card icon data successfully
    // foreach (ResourceData resource in _activePlayer.activeResources)
    // {
    //   _Cards = _resourceIconTemplate.Instantiate<TextureRect>();
    //   _Cards = resource.ResourceIcon;
    //   _resourceGridContainer.AddChild(_Cards);
    // }
  }
}
