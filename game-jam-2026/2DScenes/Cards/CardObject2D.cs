using Godot;

public partial class CardObject2D : Control
{
  [Signal] public delegate void CardSelectedEventHandler(CardData selectedData);
  [Export] public CardData AssignedCardData;

  private Label _cardNameLabel;
  public override void _Ready()
  {
    _cardNameLabel = GetNode<Label>("Item");
    if(AssignedCardData != null)
    {
      _cardNameLabel.Text = AssignedCardData.CardName;
    }
  }

  // Test to make sure the function is compatible with grid container
  public void _on_button_pressed()
  {
    if(AssignedCardData != null)
    {
      EmitSignal(SignalName.CardSelected, Variant.From(AssignedCardData));
      QueueFree();
    }
  }
}
