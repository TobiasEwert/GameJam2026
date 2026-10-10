using Godot;
using System;

[GlobalClass]
public partial class GameConfig : Resource
{
	[Export] public int startingCurrency {get; set; } // the amount of money players start with
	[Export] public int maxRounds {get; set; } // the max amount of rounds in the game
	[Export] public int baseSpinCost {get; set; } // the base cost for spinning, 
	// maybe make the first spin free or reset the spin cost to this at the beginning of each turn
	// make sure you can't be stuck if you have not enough money to spend so either free spin or make the first spin cost low
} 
