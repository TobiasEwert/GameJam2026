// WheelRoot.cs
using Godot;

[GlobalClass]
public partial class WheelRoot : Node2D
{
    [Signal] public delegate void RefreshAllButtonsEventHandler();
    [Export] Wheel wheel;
	[Export] public Button spinButton;
	[Export] public Button bankButton;
	[Export] public Label spinCost;
    public TurnManager turnManager;

    Color freeSpin = new Color("#3FBF5F");
    Color paidSpin = new Color("#FFF4DC");
    Color tooExpensive = new Color("#FF4D4D");

	//this.LabelSettings.FontColor = finalRound;

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
        EmitSignal(SignalName.RefreshAllButtons);
        if(spinCost != null)
        {
            spinCost.Text = $"Spin: ${turnManager.nextSpinCost}";
            if(turnManager.nextSpinCost == 0)
            {
                spinCost.LabelSettings.FontColor = freeSpin;
            }
            else if(turnManager.nextSpinCost <= turnManager.currentPlayer.currency)
            {
                spinCost.LabelSettings.FontColor = paidSpin;
            }
            else
            {
                spinCost.LabelSettings.FontColor = tooExpensive;
            }
        }
        if(spinButton != null)
        {
            spinButton.Disabled = turnManager.currentState != TurnManager.TurnState.PlayerAction;
        }
        if(bankButton != null)
        {
            bankButton.Disabled = turnManager.currentState != TurnManager.TurnState.PlayerAction;
        }
    }
	private void OnSpinButtonPressed()
	{       
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