using Godot;
using Godot.Collections;

public partial class MainGameLevel : Node2D
{
  [Export] public int TotalRounds = 10;
  [Export] private CardObject2D _cardTemplate;

  private PlayerData _player1;
  private PlayerData _player2;
  private PlayerData _activePlayer;

  private int _currentRound = 10;
  private bool _Player1Turn = true;

  private Label _CurrentMoney;
  private Label _RoundMoney;
  private Label _RoundNumber;
  private Label _PlayerName;
  private Wheel _Wheel;

  private TextureRect _Cards;
  private GridContainer _CardInventory;

  public override void _Ready()
  {
    _CurrentMoney = GetNode<Label>("Current");
    _RoundMoney = GetNode<Label>("RoundMoney");
    _RoundNumber = GetNode<Label>("Round");
    _PlayerName = GetNode<Label>("PlayerName");
    _Wheel = GetNode<Wheel>("Wheel");

    _Cards = GetNode<TextureRect>("TextureRect");
    _CardInventory = GetNode<GridContainer>("TextureRect/GridContainer");

    _Wheel.SpinFinished += OnActivePlayerSpinFinished;

    _player1 = new PlayerData {playerName = "Player 1", currency = 100, luck = 1.0f};
    _player2 = new PlayerData {playerName = "Player 2", currency = 100, luck = 1.0f};

    StartRound();
  }

  private void StartRound()
  {
    _Player1Turn = true;
    SetupTurnEnvironment(_player1);
  }

  private void SetupTurnEnvironment(PlayerData player)
  {
    _activePlayer = player;
    _activePlayer.roundEarnings = 0;

    UpdateUIElements();
    RebuildCardInventoryDisplay();
  }

  public void UpdateUIElements()
  {
    _PlayerName.Text = _activePlayer.PlayerName;
    _CurrentMoney.Text = $"$ {activePlayer.currency}";
    _RoundNumber.Text = $"Rounds Remaining: {_currentRoundCount}";
  }

  // private void OnActivePlayerSpinFinished(Wedge winningWedge)
  // {

  // {
  // Calls EndTurn After

  // private void RebuildCardInventoryDisplay()
  // {

  // }

  // private void EndTurn()
  // {

  // }
}
