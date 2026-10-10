using Godot;
using Godot.Collections;

public partial class MainGameLevel : Node2D
{
  [Export] private CardObject2D _cardTemplate;
  private Label _PlayerName;
  private PlayerData _activePlayer;
  public TurnManager _Turn;

  public override void _Ready()
  {
    //_PlayerName = GetNode<Label>("PlayerName");
 _Turn = TurnManager.Instance;
    // _Turn.StartGame();
  }

  public void UpdateUIElements()
  {
    //_PlayerName.Text = _activePlayer.PlayerName;

  }


}
