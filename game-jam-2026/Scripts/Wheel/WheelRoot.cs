// WheelRoot.cs
using Godot;

public partial class WheelRoot : Node2D
{
    [Export] Wheel wheel;
	[Export] public Button spinButton;
	[Export] public Button bankButton;
    public TurnManager turnManager;



    public override void _Ready()
    {
		spinButton.Pressed += OnSpinButtonPressed;
		bankButton.Pressed += OnBankButtonPressed;
        wheel.SpinFinished += OnSpinFinished;
        turnManager = TurnManager.Instance;
        turnManager.TurnStarted += OnTurnStarted;
        turnManager.StartGame();
        RefreshButtons();
    }
    public void RefreshButtons()
    {
        spinButton.Disabled = turnManager.currentState != TurnManager.TurnState.PlayerAction;
        bankButton.Disabled = turnManager.currentState != TurnManager.TurnState.PlayerAction;
    }
	private void OnSpinButtonPressed()
	{       
        //TESTING, ADD TRY PAY SPIN AND REMOVE START GAME
        if(turnManager.TryPaySpin())
        {
		    wheel.Spin();
        }
        RefreshButtons();

	}

	private void OnBankButtonPressed()
	{        
		TurnManager.Instance.BankEarnings();
        RefreshButtons();
	}

	private void OnSpinFinished(Wedge wedge)
	{
        GD.Print("Spin finished with wedge: ", wedge.Type);
		TurnManager.Instance.WedgeAction(wedge);
        RefreshButtons();
	}

	private void OnTurnStarted(int playerIndex)
	{
        RefreshButtons();
	}

}