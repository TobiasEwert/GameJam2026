// StockGraph.cs (on a Control; give it a Custom Minimum Size like 400 x 200)
using Godot;
using System.Collections.Generic;

public partial class StockGraph : Control
{
    [Export] public Color UpColor = new("3fbf5f");
    [Export] public Color DownColor = new("e04848");
    [Export] public Color GridColor = new(1, 1, 1, 0.15f);
    [Export] public float LineWidth = 3f;
    [Export] public float Padding = 12f;

    public override void _Draw()
    {
        List<float> history = Stock.Instance?.History;
        if (history == null || history.Count == 0) 
        {  
            return;
        }

        // Find the range
        float min = float.MaxValue, max = float.MinValue;
        foreach (var v in history)
        {
            min = Mathf.Min(min, v); max = Mathf.Max(max, v); 
        }
        if (Mathf.IsEqualApprox(min, max)) 
        {
            min -= 1f; max += 1f; 
        }

        Rect2 area = new Rect2(Vector2.One * Padding, Size - Vector2.One * Padding * 2f);

        // Grid lines
        for (int g = 0; g <= 4; g++)
        {
            float y = area.Position.Y + area.Size.Y * g / 4f;
            DrawLine(new Vector2(area.Position.X, y), new Vector2(area.End.X, y), GridColor, 1f);
        }

        // Price line
        var pts = new Vector2[history.Count];
        for (int i = 0; i < history.Count; i++)
        {
            float x = history.Count == 1 ? area.Position.X + area.Size.X / 2f  : area.Position.X + area.Size.X * i / (history.Count - 1);
            // 0 = lowest, 1 = highest
            float t = (history[i] - min) / (max - min);
            // flip: higher price = higher on screen
            float y = area.End.Y - area.Size.Y * t;
            pts[i] = new Vector2(x, y);
        }

        bool up = history.Count < 2 || history[^1] >= history[^2];
        var color = up ? UpColor : DownColor;

        if (pts.Length > 1) 
        {
            DrawPolyline(pts, color, LineWidth, true);
        }   
        foreach (var p in pts) 
        {
            DrawCircle(p, LineWidth + 1f, color);
        }
    }
}