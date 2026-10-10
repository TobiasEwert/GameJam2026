using Godot;
using Godot.Collections;

public partial class MainGameLevel : Node2D
{
  private Label _PlayerName;
  private PlayerData _activePlayer;
  public TurnManager _Turn;
  [Export] public CardData [] allCards;

  public override void _Ready()
  {
    //_PlayerName = GetNode<Label>("PlayerName");
 _Turn = TurnManager.Instance;
 _Turn.possibleCards = allCards;
    // _Turn.StartGame();
  }

  public void UpdateUIElements()
  {
    //_PlayerName.Text = _activePlayer.PlayerName;

  }


}
