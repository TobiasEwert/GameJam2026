using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class StockScreen : Control
{    
    [Signal] public delegate void StockScreenOpenedEventHandler();
    [Signal] public delegate void StockScreenClosedEventHandler();
    [Export] public StockGraph graph;
    [Export] public Label priceLabel;
    [Export] public Label ownedLabel;
    [Export] public Label previewLabel;
    [Export] public HSlider amountSlider;
    [Export] public Button buyButton;
    [Export] public Button sellButton;
    [Export] public Button closeButton;
    public bool bIsOpen = false;
    // Lambda
    public void Close()
    {
        Visible = false;
        bIsOpen = false;
        EmitSignal(nameof(StockScreenClosed));
        turnManager.currentState = TurnManager.TurnState.PlayerAction;
    }

    void OnPriceChanged(float newValue, float oldValue) => Refresh();
    void OnSharesChanged(int playerIndex, float shares) => Refresh();
    void OnMoneyChanged(int playerIndex, int money) => Refresh();
    public TurnManager turnManager;
    public Stock stock;

    public override void _Ready()
    {
        turnManager = TurnManager.Instance;
        stock = Stock.Instance;

        stock.PriceChanged += OnPriceChanged;
        stock.SharesChanged += OnSharesChanged;
        turnManager.MoneyChanged += OnMoneyChanged;

        amountSlider.ValueChanged += _ => UpdatePreview();
        buyButton.Pressed += OnBuyPressed;
        sellButton.Pressed += () => stock.SellAllStocks(turnManager.currentPlayer);
        closeButton.Pressed += Close;

        Visible = false;
    }

    public override void _ExitTree()
    {
        stock.PriceChanged -= OnPriceChanged;
        stock.SharesChanged -= OnSharesChanged;
        turnManager.MoneyChanged -= OnMoneyChanged;
    }

    public void Open()
    {
        if(bIsOpen)
        {
            Visible = false;
            bIsOpen = false;
            EmitSignal(nameof(StockScreenClosed));
            turnManager.currentState = TurnManager.TurnState.PlayerAction;
            return;
        }
        if (turnManager.currentState != TurnManager.TurnState.PlayerAction)
        {
            return;
        }
        bIsOpen = true;
        Visible = true;
        EmitSignal(nameof(StockScreenOpened));
        Refresh();
    }


    void OnBuyPressed()
    {
        if (stock.BuyStock(turnManager.currentPlayer, (int)amountSlider.Value))
        {
            amountSlider.Value = 0;
        }
    }

    void Refresh()
    {
        if (!Visible || turnManager.currentPlayer == null)
        {
            return;
        }

        PlayerData player = turnManager.currentPlayer;
        List<float> history = stock.History;
        float change = history.Count > 1 ? stock.CurrentValue - history[^2] : 0f;
        string arrow = change >= 0 ? "▲" : "▼";

        priceLabel.Text = $"Price: ${stock.CurrentValue:0.00}  {arrow} {Mathf.Abs(change):0.00}";
        ownedLabel.Text = $"You own {player.stockShares:0.##} shares (worth ${stock.ValueOf(player):0})";

        amountSlider.MinValue = 0;
        amountSlider.MaxValue = Mathf.Max(0, player.currency - 1);
        amountSlider.Step = 1;

        sellButton.Disabled = player.stockShares <= 0f;
        graph.QueueRedraw();
        UpdatePreview();
    }

    void UpdatePreview()
    {
        int amount = (int)amountSlider.Value;
        previewLabel.Text = amount == 0 ? "Choose an amount to invest" : $"Invest ${amount} → about {amount / stock.CurrentValue:0.##} shares";
        buyButton.Disabled = amount == 0;
    }

}