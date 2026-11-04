using System.Collections.ObjectModel;

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
        foreach (string line in File.ReadLines(userGoalDataFile))
        {
            if (String.IsNullOrWhiteSpace(line)) { continue; }
            if (line.Split(',') is string[] lineData && lineData.Length == 4)
            {
                uint AmmountPrimary = uint.Parse(lineData[1]);
                byte AmountSecondary = byte.Parse(lineData[2]);
                DateTime DateTimeOfItem = DateTime.FromBinary(long.Parse(lineData[3]));

                if (AmountSecondary / 100 == 0) { TransactionHistory[DateTimeOfItem.Date] += AmmountPrimary * 100 + AmountSecondary; }
                else { TransactionHistory[DateTimeOfItem.Date] -= (AmmountPrimary * 100) + (AmountSecondary % 100); }
            }
        }

        DailyGoals.Clear();
        CustomGoals.Clear();
        foreach (string line in File.ReadLines(userGoalDataFile))
        {
            if (String.IsNullOrWhiteSpace(line)) { continue; }
            if (line.Split(',') is string[] lineData && (lineData.Length == 4 || lineData.Length == 5))
            {
                if (lineData[0] == "daily")
                {
                    DailyGoal temp = new() {
                        Message = lineData[1],
                        GoalCashString = lineData[2],
                        DateOfGoal = DateTime.FromBinary(long.Parse(lineData[3]))
                    };

                    DailyGoals.Add(temp);
                }
                else if (lineData[0] == "custom")
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
            long total = TransactionHistory[goal.DateOfGoal.Date];
            if (total > 0) { goal.RealCashString = $"$+{total / 100}.{total % 100}"; }
            else if (total < 0) { goal.RealCashString = $"${total / 100}.{Math.Abs(total) % 100}"; }
            else { goal.RealCashString = "$0.00"; }
        }

        foreach (CustomGoal goal in CustomGoals)
        {
            long total = 0;

            for (DateTime current = goal.DateTimeStart; current.Date <= goal.DateTimeEnd.Date; current.AddDays(1))
            {
                total += TransactionHistory[current.Date];
            }

            if (total > 0) { goal.RealCashString = $"$+{total / 100}.{total % 100}"; }
            else if (total < 0) { goal.RealCashString = $"${total / 100}.{Math.Abs(total) % 100}"; }
            else { goal.RealCashString = "$0.00"; }
        }
    }
}