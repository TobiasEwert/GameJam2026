using Godot;
using System;

public partial class WedgeFeedback : Node
{
	[Export] public Button spinButton;
	[Export] public Button bankButton;
	public TurnManager turnManager;

	public override void _Ready()
	{
		turnManager = TurnManager.Instance;
		turnManager.WedgeResolved += OnWedgeResolved;
	}
	void OnWedgeResolved(Wedge wedge, int oldPot, int newPot)
	{
		spinButton.Disabled = true;
		bankButton.Disabled = true;
		switch (wedge.Type)
		{
			case Wedge.WedgeType.Bust:
			   PlayBust(oldPot); 
			   break; 
			case Wedge.WedgeType.Double:
				PlayMultiplier("x2"); 
				break;
			case Wedge.WedgeType.Triple: 
				PlayMultiplier("x3"); 
				break;
			case Wedge.WedgeType.Add:    
				PlayPopup($"+${wedge.Amount}"); 
				break;
			default:                     
				PlayPopup(wedge.Label); 
				break;
		}
	}

	public void PlayBust(int oldPot)
	{
		// ui/vfx

		//At The end
		spinButton.Disabled = false;
		bankButton.Disabled = false;
	}

	public void PlayMultiplier(string multiplier)
	{
		// ui/vfx
		//At The end
		spinButton.Disabled = false;
		bankButton.Disabled = false;
	}

	public void PlayPopup(string message)
	{
		// ui/vfx
		//At The end
		GD.Print("Popup message: ", message);
		spinButton.Disabled = false;
		bankButton.Disabled = false;
	}
}
