using Godot;

public partial class WheelShadow : Node2D
{
    [Export] public Wheel wheel;
    [Export] public Vector2 Offset = new(6, 8);
    [Export] public Color ShadowColor = new(0, 0, 0, 0.4f);

    public override void _Draw()
    {
        float radius = wheel != null ? wheel.Radius + wheel.RimWidth : 214f;
        DrawCircle(Offset, radius, ShadowColor);
    }
}