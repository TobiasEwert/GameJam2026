// WheelRoot.cs
using Godot;

public partial class WheelRoot : Node2D
{
    Wheel wheel;
    //button ref, maybe make these [Export] so asssigning them is simpler,
    //on button press, call wheel.Spin()
    //Call TurnManager TryPaySpin() first 

    public override void _Ready()
    {
        wheel = GetNode<Wheel>("Wheel");

		wheel.Spin();
    }



}