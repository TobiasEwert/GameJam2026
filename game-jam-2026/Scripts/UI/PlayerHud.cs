using Godot;
using System;

public partial class PlayerHud : HBoxContainer
{
public TurnManager turnManager;
public Stock stock;
[Export] public PanelContainer p1Panel;
[Export] public Label p1PlayerName;
[Export] public Label p1Money;
[Export] public Label p1Stock;
[Export] public Label p2PlayerName;
[Export] public Label p2Money;
[Export] public Label p2Stock;

[Export] public PanelContainer p2Panel;
[Export] public Label p1Turn;
[Export] public Label p2Turn;


StyleBoxFlat activeStyle, inactiveStyle;

public override void _Ready()
{
    turnManager = TurnManager.Instance;
    stock = Stock.Instance;
    stock.SharesChanged += OnSharesChanged;
	turnManager.MoneyChanged += OnMoneyChanged;
	turnManager.TurnStarted += OnTurnStarted;
	p1PlayerName.Text = turnManager.players[0].playerName;
	p2PlayerName.Text = turnManager.players[1].playerName;
    activeStyle = MakePanelStyle(GameColors.Gold, 3);
    inactiveStyle = MakePanelStyle(new Color("5a4a66"), 1);
    RefreshAll();
}

private void RefreshAll()
{
	OnMoneyChanged(0, turnManager.players[0].currency);
	OnMoneyChanged(1, turnManager.players[1].currency);
	HighlightPlayer(turnManager.currentPlayerIndex);
}

private void OnMoneyChanged(int playerIndex, int money)
{
	if (playerIndex == 0)
		{
			p1Money.Text = $"{money}";
		}
	else if (playerIndex == 1)
		{
			p2Money.Text = $"{money}";
		} 
}
private void OnSharesChanged(int playerIndex, float shares)
{
    if (playerIndex == 0)
    {
        p1Stock.Text = $"Shares: {shares:F2}";
    }
    else if (playerIndex == 1)
    {
        p2Stock.Text = $"Shares: {shares:F2}";
    }
}
private void OnTurnStarted(int playerIndex)
{
	OnMoneyChanged(playerIndex, turnManager.currentPlayer.currency);
	HighlightPlayer(playerIndex);
}

StyleBoxFlat MakePanelStyle(Color border, int borderWidth)
{
    var style = new StyleBoxFlat
    {
        BgColor = GameColors.Panel,
        BorderColor = border,
        ContentMarginLeft = 14, ContentMarginRight = 14,
        ContentMarginTop = 10, ContentMarginBottom = 10
    };
    style.SetBorderWidthAll(borderWidth);
    style.SetCornerRadiusAll(10);
    return style;
}

void HighlightPlayer(int index)
{
    bool p1Active = index == 0;

    p1Panel.AddThemeStyleboxOverride("panel", p1Active ? activeStyle : inactiveStyle);
    p2Panel.AddThemeStyleboxOverride("panel", p1Active ? inactiveStyle : activeStyle);

    p1Panel.Modulate = new Color(1, 1, 1, p1Active ? 1f : 0.8f);
    p2Panel.Modulate = new Color(1, 1, 1, p1Active ? 0.8f : 1f);

	Color activeGold = new Color("#E8B84A");
	Color inactiveCream = new Color("#FFF4DC");

	p1Money.LabelSettings.FontColor = p1Active ? activeGold : inactiveCream;
	p2Money.LabelSettings.FontColor = p1Active ? inactiveCream : activeGold;

    p1Turn.Visible = p1Active;
    p2Turn.Visible = !p1Active;
}

}
