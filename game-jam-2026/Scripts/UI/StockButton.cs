using Godot;
using System;

public partial class StockButton : Button
{
    public TurnManager turnManager;
    public override void _Ready()
    {
        base._Ready();
        turnManager = TurnManager.Instance;
    }

    
}
