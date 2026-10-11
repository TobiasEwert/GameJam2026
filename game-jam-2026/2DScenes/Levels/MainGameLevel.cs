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
  [Export] public WheelRoot wheelRoot;
  [Export] public AudioStreamPlayer2D backgroundMusicPlayer;
  [Export] public AudioStreamMP3 gameOverMusic;
  [Export] public AudioStreamPlayer2D audioPlayer;
  [Export] public Label popupLabel;
  private Timer popupTimer;

  public override void _Ready()
  {
    _Turn = TurnManager.Instance;
    _Turn.possibleCards = allCards;
    _Turn.GameEnded += OnGameEnded;
    _Turn.PopupMessage += OnPopupMessage;
    gameOverScreen.Visible = false;
    wheelRoot.RefreshAllButtons += OnRefreshButtons;

    popupTimer = new Timer();
    popupTimer.OneShot = true;
    popupTimer.WaitTime = 5.0;
    AddChild(popupTimer);
    popupTimer.Timeout += HidePopup;

    if (popupLabel != null)
    {
      popupLabel.Visible = false;
    }

    if (_Turn.currentPlayer != null)
    {
      ShowPopup($"{_Turn.currentPlayer.playerName}'s TURN");
    }
  }

  private void OnPopupMessage(string message)
  {
    ShowPopup(message);
  }

  public void ShowPopup(string message)
  {
    if (popupLabel == null)
    {
      GD.PushWarning("MainGameLevel: popupLabel is not assigned. Message: " + message);
      return;
    }
    popupLabel.Text = message;
    popupLabel.Visible = true;
    popupTimer.Start();
  }

  private void HidePopup()
  {
    if (popupLabel == null)
    {
      return;
    }
    popupLabel.Visible = false;
    popupLabel.Text = "";
  }

  private void OnRefreshButtons()
  {
  }

  public void MainLevelUIController(bool visible)
  {
    foreach (var uiElement in mainGameUI)
    {
      uiElement.Visible = visible;
    }
    wheelRoot.Visible = visible;
    ((Node2D)wheelRoot.GetParent().GetParent()).Visible = visible;
  }

  private void OnGameEnded(int winnerIndex)
  {
    MainLevelUIController(false);
    gameOverScreen.Visible = true;
    audioPlayer.Stream = gameOverMusic;
    audioPlayer.Play();
  }

}
