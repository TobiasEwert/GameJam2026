using Godot;
using System;

[GlobalClass]
public partial class PlayerData : Resource
{
    [Export] public int currency {get; set; } // the money
    [Export] public float luck { get; set; } 
    [Export] public string playerName {get; set; } // we could allow them to set their own name, else we just force player 1 and player 2 as the names
    [Export] public int roundEarnings {get; set; } // the earnings for the current round, positive or negative, this will be set to zero when your turn  begins
    [Export] public int stockShares {get; set; } // the amount of stock shares a player owns

    //[Export] public <whatever the ability card class is called or string> [] abilityCards;
    // Can also be a string assigned to each card and we do a switch statement to determine the effects
    
}
