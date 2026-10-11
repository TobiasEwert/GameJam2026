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

    const float PointerAngle = 0;

    readonly RandomNumberGenerator rng = new();
    float[] starts, ends; 
    bool spinning;
    int lastIndexUnderPointer = -1;
    [Export] public Font customFont;

    // Line outline for each wedge
    [Export] public Color LineColor = Colors.Black;
    [Export] public float LineWidth = 3f;

    // Center circle
    [Export] public float CapRadius = 30f;
    [Export] public Color CapRimColor = new("e8b84a");
    [Export] public Color CapInnerColor = new("2a1a3a");
    [Export] public Color CapShineColor = new(1, 1, 1, 0.35f);

    // Wheel outline
    [Export] public float RimWidth = 14f;
    [Export] public Color RimColor = new("e8b84a");
    [Export] public Color RimEdgeColor = new("a9782a");
    [Export] public float PegRadius = 5f;
    [Export] public Color PegColor = new("fff4dc");

    // Pointer 
    [Export] public Node2D Pointer;
    [Export] public float KickDegrees = 18f;
    [Export] public float KickReturnTime = 0.18f;
    [Export] public float KickDirection = -1f;
    Tween pointerTween;



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

            float mid = (starts[i] + ends[i]) / 2f;
            DrawSetTransform(Vector2.FromAngle(mid) * Radius * 0.35f, mid, Vector2.One);
            Font font = ThemeDB.FallbackFont;
            Vector2 labelPos = new Vector2(-24, 6);
            int fontSize = 18;
            int outlineSize = 5;

            DrawStringOutline(font, labelPos, Wedges[i].Label, HorizontalAlignment.Center, 96, fontSize, outlineSize, GameColors.Ink);
            DrawString(font, labelPos, Wedges[i].Label, HorizontalAlignment.Center, 96, fontSize, Wedges[i].Type == Wedge.WedgeType.Bust ? GameColors.BustText : GameColors.Cream);
            Vector2 center = Vector2.FromAngle(mid) * Radius * 0.8f;
            float iconSize = 36f; 

            DrawSetTransform(center, mid, Vector2.One);
            DrawTextureRect(Wedges[i].Icon, new Rect2(-Vector2.One * iconSize / 2f, Vector2.One * iconSize), false, null, false);
            DrawSetTransform(Vector2.Zero, 0f, Vector2.One);   
            GD.Print("Drawing wedge ", i);
        }
        // Wedge Outline 
        // Rim
        float rimMid = Radius + RimWidth / 2f;
        DrawArc(Vector2.Zero, rimMid, 0f, Mathf.Tau, 96, RimColor, RimWidth, true);

        // Edges
        DrawArc(Vector2.Zero, Radius, 0f, Mathf.Tau, 96, RimEdgeColor, 2f, true);
        DrawArc(Vector2.Zero, Radius + RimWidth, 0f, Mathf.Tau, 96, RimEdgeColor, 2f, true);

        // Pegs
        for (int i = 0; i < Wedges.Count; i++)
        {
            Vector2 pegPos = Vector2.FromAngle(starts[i]) * rimMid;
            DrawCircle(pegPos, PegRadius + 1.5f, LineColor);
            DrawCircle(pegPos, PegRadius, PegColor);
            DrawCircle(pegPos + new Vector2(-1.5f, -1.5f), PegRadius * 0.35f, new Color(1, 1, 1, 0.6f));
        }

        // Center cap
        DrawCircle(new Vector2(2, 3), CapRadius + 2f, new Color(0, 0, 0, 0.35f));
        DrawCircle(Vector2.Zero, CapRadius, CapRimColor);
        DrawCircle(Vector2.Zero, CapRadius * 0.7f, CapInnerColor);
        DrawCircle(new Vector2(-CapRadius * 0.25f, -CapRadius * 0.25f), CapRadius * 0.18f, CapShineColor);
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
            KickPointer();
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

    void KickPointer()
    {
        if (Pointer == null) return;

        pointerTween?.Kill();

        Pointer.Rotation = Mathf.DegToRad(KickDegrees * KickDirection);

        pointerTween = CreateTween();
        pointerTween.TweenProperty(Pointer, "rotation", 0f, KickReturnTime).SetTrans(Tween.TransitionType.Elastic).SetEase(Tween.EaseType.Out);
    }
}