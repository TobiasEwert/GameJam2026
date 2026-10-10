using Godot;

[GlobalClass]
public partial class CardData : Resource
{
  [Export] public string CardName { get; set; }
  [Export] public string Description {get; set; }
  [Export] public Texture2D Icon { get; set; }
  [Export] public int ActionId {get; set; }
}
