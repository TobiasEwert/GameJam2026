// Wedge.cs
using Godot;

[GlobalClass]
public partial class Wedge : Resource
{
		public enum WedgeType
	{
		Bust,
		BreakEven,
		Lose,
		Double,
		Triple,
		Half,
		Add,
		AbilityCard
		// New wedge types are added here
	}
    [Export] public string Label = "+$20";
	[Export] public Texture2D Icon;
    [Export] public WedgeType Type = WedgeType.Add;
    [Export] public int Amount = 20;
    [Export] public float Weight = 1f;
    [Export] public Color Color = Colors.Green;
}