// WheelRoot.cs
using Godot;

public partial class WheelRoot : Node2D
{
    Wheel _wheel;


    public override void _Ready()
    {
        _wheel = GetNode<Wheel>("Wheel");



		        _wheel.Spin();
    }



}