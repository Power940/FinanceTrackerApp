using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Windows.Services.Maps;

namespace PotatoUnitedCreditUnion;

public partial class TransactionHistoryPage : ContentPage
{
    string userTransactionHistoryDataFile = "";
    public ObservableCollection<TransactionItem> TransactionHistory { get; set; } = [];

    public TransactionHistoryPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        SetupData();
    }

    private void SetupData()
    {
        BindingContext = this;
        userTransactionHistoryDataFile = Path.Combine(MainPage.USER_DIR_PATH, "history.csv");
        if (!File.Exists(userTransactionHistoryDataFile))
        {
            File.Create(userTransactionHistoryDataFile).Dispose();
        }

        TransactionHistory.Clear();
        foreach (string line in File.ReadLines(userTransactionHistoryDataFile))
        {
            if (String.IsNullOrWhiteSpace(line)) { continue; }
            if (line.Split(',') is string[] lineData && lineData.Length == 4)
            {
                TransactionItem temp = new()
                {
                    Message = lineData[0],
                    AmmountPrimary = uint.Parse(lineData[1]),
                    AmountSecondary = byte.Parse(lineData[2]),
                    DateTimeOfItem = DateTime.FromBinary(long.Parse(lineData[3]))
                };
                temp.UpdateCashString();

                TransactionHistory.Add(temp);
            }
        }
        SortAndUpdateTotalTransactionHistory();
    }

    private void OnNumericTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) { return; }
        if (string.IsNullOrEmpty(e.NewTextValue)) return;
        if (!Regex.IsMatch(e.NewTextValue, @"^[0-9]*$")) { entry.Text = e.OldTextValue; }
    }

    private void NoCommaTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is not Entry entry) { return; }
        if (string.IsNullOrEmpty(e.NewTextValue)) return;
        if (Regex.IsMatch(e.NewTextValue, @",")) { entry.Text = e.OldTextValue; }
    }

    private async void Back_BTN_Clicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void Create_BTN_Clicked(object? sender, EventArgs e)
    {
        try
        {
            TransactionItem temp = new()
            {
                Message = message_ENT.Text,
                AmmountPrimary = uint.Parse(primary_ENT.Text),
                AmountSecondary = (byte)(byte.Parse(secondary_ENT.Text) + (negative_SW.IsToggled ? 100 : 0)),
                DateTimeOfItem = (date_DP.Date ?? DateTime.Today) + (time_TP.Time ?? DateTime.Now.TimeOfDay)
            };
            temp.UpdateCashString();

            TransactionHistory.Add(temp);
            scrollableTransactionItems_CV.ScrollTo(TransactionHistory.Count() - 1);
            SortAndUpdateTotalTransactionHistory();
            SaveTransactionHistory();
        }
        catch (Exception)
        {
            return;
        }
    }

    private void Delete_BTN_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) { return; }
        if (button.BindingContext is not TransactionItem item) { return; }
        TransactionHistory.Remove(item);
        SortAndUpdateTotalTransactionHistory();
        SaveTransactionHistory();
    }

    private void Negative_SW_Toggled(object? sender, ToggledEventArgs e)
    {
        if (e.Value) { negative_LB.Text = "-"; } else { negative_LB.Text = "+"; }
    }

    private void SaveTransactionHistory()
    {
        File.WriteAllLines(userTransactionHistoryDataFile, TransactionHistory.Select(item => item.GetCSVString()));
    }

    private void SortAndUpdateTotalTransactionHistory()
    {
        long total = 0;
        var sortedTransactions = TransactionHistory.OrderBy(item => item.DateTimeOfItem).ToList();
        TransactionHistory.Clear();
        foreach (var item in sortedTransactions)
        {
            TransactionHistory.Add(item);
            total += item.ToLong();
        }
        total_LB.Text = $"${total / 100}.{Math.Abs(total % 100).ToString().PadLeft(2, '0')}";
        if (total > 0) { total_LB.TextColor = Colors.Green; }
        else if (total < 0) { total_LB.TextColor = Colors.Red; }
        else { total_LB.TextColor = Colors.White; }
    }
}