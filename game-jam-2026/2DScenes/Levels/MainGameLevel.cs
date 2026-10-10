using Godot;
using Godot.Collections;

public partial class MainGameLevel : Node2D
{
  private Label _PlayerName;
  private PlayerData _activePlayer;
  public TurnManager _Turn;
  [Export] public CardData [] allCards;
  [Export] public Control gameOverScreen;
  [Export] public Control [] mainGameUI;
  [Export] public Node2D wheel;
  [Export] public AudioStreamMP3 gameOverMusic;
  [Export] public AudioStreamPlayer2D audioPlayer;

  public override void _Ready()
  {
    //_PlayerName = GetNode<Label>("PlayerName");
    _Turn = TurnManager.Instance;
    _Turn.possibleCards = allCards;
    _Turn.GameEnded += OnGameEnded;
    gameOverScreen.Visible = false;
    // _Turn.StartGame();
  }

  public void MainLevelUIController(bool visible)
  {
    foreach (var uiElement in mainGameUI)
    {
      uiElement.Visible = visible;
    }
    wheel.Visible = visible;
  }

  private void OnGameEnded(int winnerIndex)
  {
    MainLevelUIController(false);
    gameOverScreen.Visible = true;
    audioPlayer.Stream = gameOverMusic;
    audioPlayer.Play();
  }

}
