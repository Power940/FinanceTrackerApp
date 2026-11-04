using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace PotatoUnitedCreditUnion;

public partial class GoalPage : ContentPage
{
    string userGoalDataFile = "";
    string userTransactionHistoryDataFile = "";
    public ObservableCollection<DailyGoal> DailyGoals { get; set; } = [];
    public ObservableCollection<CustomGoal> CustomGoals { get; set; } = [];
    public Dictionary<DateTime, long> TransactionHistory { get; set; } = [];

    public GoalPage()
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
        userGoalDataFile = Path.Combine(MainPage.USER_DIR_PATH, "goal.csv");
        if (!File.Exists(userGoalDataFile))
        {
            File.Create(userGoalDataFile).Dispose();
        }
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
                long AmmountPrimary = uint.Parse(lineData[1]);
                byte AmountSecondary = byte.Parse(lineData[2]);
                DateTime DateTimeOfItem = DateTime.FromBinary(long.Parse(lineData[3]));

                if (!TransactionHistory.ContainsKey(DateTimeOfItem.Date)) { TransactionHistory[DateTimeOfItem.Date] = 0; }

                if (AmountSecondary / 100 == 0) { TransactionHistory[DateTimeOfItem.Date] += AmmountPrimary * 100 + AmountSecondary; }
                else { TransactionHistory[DateTimeOfItem.Date] -= (AmmountPrimary * 100) + (AmountSecondary % 100); }
            }
        }

        DailyGoals.Clear();
        CustomGoals.Clear();
        foreach (string line in File.ReadLines(userGoalDataFile))
        {
            if (String.IsNullOrWhiteSpace(line)) { continue; }
            if (line.Split(',') is string[] lineData)
            {
                if (lineData[0] == "daily" && lineData.Length == 4)
                {
                    DailyGoal temp = new() {
                        Message = lineData[1],
                        GoalCashString = lineData[2],
                        DateOfGoal = DateTime.FromBinary(long.Parse(lineData[3]))
                    };

                    DailyGoals.Add(temp);
                }
                else if (lineData[0] == "custom" && lineData.Length == 5)
                {
                    CustomGoal temp = new() {
                        Message = lineData[1],
                        GoalCashString = lineData[2],
                        DateTimeStart = DateTime.FromBinary(long.Parse(lineData[3])),
                        DateTimeEnd = DateTime.FromBinary(long.Parse(lineData[4]))
                    };

                    CustomGoals.Add(temp);
                }
            }
        }

        UpdateAllRealCashStrings();
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

    private void CustomOrDaily_SW_Toggled(object? sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            customOrDaily_LB.Text = "Mode:\nDaily";
            dateEnd_DP.IsEnabled = false;
            dateEnd_DP.IsVisible = false;
        }
        else
        {
            customOrDaily_LB.Text = "Mode:\nCustom";
            dateEnd_DP.IsEnabled = true;
            dateEnd_DP.IsVisible = true;
        }
    }

    private async void Back_BTN_Clicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void Delete_BTN_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) { return; }
        if (button.BindingContext is DailyGoal dGoal) { DailyGoals.Remove(dGoal); }
        else if (button.BindingContext is CustomGoal cGoal) { CustomGoals.Remove(cGoal); }
        else { return; }
        SaveGoals();
    }

    private void Duplicate_BTN_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) { return; }
        if (button.BindingContext is not DailyGoal goal) { return; }

        DailyGoal temp = new() {
            Message = goal.Message,
            GoalCashString = goal.GoalCashString,
            DateOfGoal = goal.DateOfGoal.AddDays(1).Date
        };

        DailyGoals.Add(temp);

        UpdateAllRealCashStrings();
        SaveGoals();
    }

    private void Create_BTN_Clicked(object? sender, EventArgs e)
    {
        if (customOrDaily_SW.IsToggled)
        {
            DailyGoal temp = new()
            {
                Message = message_ENT.Text,
                GoalCashString = $"$+{primary_ENT.Text ?? "0"}.{(secondary_ENT.Text ?? "00").PadLeft(2, '0')}",
                DateOfGoal = dateStart_DP.Date ?? DateTime.Today
            };
            if (int.Parse(primary_ENT.Text ?? "0") == 0 && int.Parse(secondary_ENT.Text ?? "0") == 0) { temp.GoalCashString = "$0.00"; }

            DailyGoals.Add(temp);
        }
        else
        {
            CustomGoal temp = new()
            {
                Message = message_ENT.Text,
                GoalCashString = $"$+{primary_ENT.Text ?? "0"}.{(secondary_ENT.Text ?? "00").PadLeft(2, '0')}",
                DateTimeStart = dateStart_DP.Date ?? DateTime.Today,
                DateTimeEnd = dateEnd_DP.Date ?? DateTime.Today.AddDays(1)
            };
            if (int.Parse(primary_ENT.Text ?? "0") == 0 && int.Parse(secondary_ENT.Text ?? "0") == 0) { temp.GoalCashString = "$0.00"; }

            CustomGoals.Add(temp);
        }

        UpdateAllRealCashStrings();
        SaveGoals();
    }

    private void SaveGoals()
    {
        List<ICSV> Goals = [];
        Goals.AddRange(DailyGoals);
        Goals.AddRange(CustomGoals);
        File.WriteAllLines(userGoalDataFile, Goals.Select(item => item.GetCSVString()));
    }

    private void UpdateAllRealCashStrings()
    {
        foreach (DailyGoal goal in DailyGoals)
        {
            long total = 0;
            if (TransactionHistory.ContainsKey(goal.DateOfGoal.Date)) { total = TransactionHistory[goal.DateOfGoal.Date]; }
            if (total > 0) { goal.RealCashString = $"$+{total / 100}.{(total % 100).ToString().PadLeft(2, '0')}"; }
            else if (total < 0) { goal.RealCashString = $"${total / 100}.{(Math.Abs(total) % 100).ToString().PadLeft(2, '0')}"; }
            else { goal.RealCashString = "$0.00"; }

            long expectedTotal = (long.Parse(goal.GoalCashString.AsSpan()[2..^3]) * 100) + long.Parse(goal.GoalCashString.AsSpan()[^2..]);
            if (goal.GoalCashString.Contains('-')) { expectedTotal *= -1; }

            if (total > expectedTotal) { goal.RealCashStringColor = Colors.Green; }
            else if (total < expectedTotal) { goal.RealCashStringColor = Colors.Red; }
            else { goal.RealCashStringColor = Colors.Black; }
        }

        foreach (CustomGoal goal in CustomGoals)
        {
            long total = 0;

            for (DateTime current = goal.DateTimeStart; current.Date <= goal.DateTimeEnd.Date; current = current.AddDays(1))
            {
                if (TransactionHistory.ContainsKey(current.Date)) { total += TransactionHistory[current.Date]; }
            }

            if (total > 0) { goal.RealCashString = $"$+{total / 100}.{(total % 100).ToString().PadLeft(2, '0')}"; }
            else if (total < 0) { goal.RealCashString = $"${total / 100}.{(Math.Abs(total) % 100).ToString().PadLeft(2, '0')}"; }
            else { goal.RealCashString = "$0.00"; }

            long expectedTotal = (long.Parse(goal.GoalCashString.AsSpan()[2..^3]) * 100) + long.Parse(goal.GoalCashString.AsSpan()[^2..]);
            if (goal.GoalCashString.Contains('-')) { expectedTotal *= -1; }

            if (total > expectedTotal) { goal.RealCashStringColor = Colors.Green; }
            else if (total < expectedTotal) { goal.RealCashStringColor = Colors.Red; }
            else { goal.RealCashStringColor = Colors.Black; }
        }
    }
}