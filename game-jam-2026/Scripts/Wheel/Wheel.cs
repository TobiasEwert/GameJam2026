using Godot;
using Godot.Collections;

public partial class Wheel : Node2D
{
    [Signal] public delegate void SpinFinishedEventHandler(Wedge wedge);

    [Export] public Array<Wedge> Wedges = new();
    [Export] public float Radius = 200f;
    [Export] public float SpinTime = 4f;
    [Export] public int MinTurns = 4;
    [Export] public AudioStreamPlayer TickSound;

    const float PointerAngle = -Mathf.Pi / 2f;

    readonly RandomNumberGenerator rng = new();
    float[] starts, ends; 
    bool spinning;
    int lastIndexUnderPointer = -1;

    public override void _Ready()
    {
        rng.Randomize();
        RebuildAngles();
    }

    public void RebuildAngles()
    {
        float total = 0f;
        foreach (var w in Wedges) total += w.Weight;

        starts = new float[Wedges.Count];
        ends = new float[Wedges.Count];
        float a = 0f;
        for (int i = 0; i < Wedges.Count; i++)
        {
            starts[i] = a;
            a += Mathf.Tau * Wedges[i].Weight / total;
            ends[i] = a;
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        for (int i = 0; i < Wedges.Count; i++)
        {
            
            var pts = new Vector2[34];
            pts[0] = Vector2.Zero;
            for (int s = 0; s <= 32; s++)
            {
                float t = Mathf.Lerp(starts[i], ends[i], s / 32f);
                pts[s + 1] = Vector2.FromAngle(t) * Radius;
            }
            DrawColoredPolygon(pts, Wedges[i].Color);

            float mid = ((starts[i] + ends[i]) / 2f);
            DrawSetTransform(Vector2.FromAngle(mid) * Radius * 0.4f, mid, Vector2.One);
            DrawString(ThemeDB.FallbackFont, new Vector2(-24, 6), Wedges[i].Label, HorizontalAlignment.Center, 96, 18);
            mid = (starts[i] + ends[i]) / 2f;
            Vector2 center = Vector2.FromAngle(mid) * Radius * 0.8f;
            float iconSize = 24f; 

            DrawSetTransform(center, mid, Vector2.One);
            DrawTextureRect(Wedges[i].Icon, new Rect2(-Vector2.One * iconSize / 2f, Vector2.One * iconSize), false, null, false);
            DrawSetTransform(Vector2.Zero, 0f, Vector2.One);   
            GD.Print("Drawing wedge ", i);

        }
    }

    public void Spin()
    {
        if (spinning || Wedges.Count == 0) 
        {
            return;
        }
        spinning = true;

        var weights = new float[Wedges.Count];
        for (int i = 0; i < Wedges.Count; i++) 
        {  
            weights[i] = Wedges[i].Weight;
        }
        int index = (int)rng.RandWeighted(weights);

        float margin = (ends[index] - starts[index]) * 0.15f;
        float landAngle = rng.RandfRange(starts[index] + margin, ends[index] - margin);

        Rotation = Mathf.PosMod(Rotation, Mathf.Tau);
        float offset = Mathf.PosMod(PointerAngle - landAngle - Rotation, Mathf.Tau);
        float target = Rotation + MinTurns * Mathf.Tau + offset;

        var tween = CreateTween();
        tween.TweenProperty(this, "rotation", target, SpinTime).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
        tween.Finished += () =>
        {
            spinning = false;
            EmitSignal(SignalName.SpinFinished, Wedges[index]);
        };
    }

    public override void _Process(double delta)
    {
        if (!spinning)
        {   
            return; 
            }

        int i = IndexUnderPointer();
        if (i != lastIndexUnderPointer)
        {
            lastIndexUnderPointer = i;
            TickSound?.Play();
        }
    }

    public int IndexUnderPointer()
    {
        float local = Mathf.PosMod(PointerAngle - Rotation, Mathf.Tau);
        for (int i = 0; i < ends.Length; i++)
        {
            if (local < ends[i]) return i;
        }
        return ends.Length - 1;
    }
}