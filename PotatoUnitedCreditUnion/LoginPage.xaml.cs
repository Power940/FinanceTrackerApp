using System.IO;
using Windows.System;
using static System.Net.Mime.MediaTypeNames;

namespace PotatoUnitedCreditUnion;

public partial class LoginPage : ContentPage
{
    static readonly string userDataFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, "userData.csv");

    private int userIdIndex;
    private int newUserIdNum;
    private string[] userLoginParts = new string[2];
    private List<int> userIds = [];

    public LoginPage()
    {
        InitializeComponent();
        SetupData();
    }

    private void SetupData()
    {
        userIdIndex = -1;
        newUserIdNum = 0;
        userLoginParts = new string[2];
        userIds = [];

        if (!File.Exists(userDataFilePath))
        {
            File.Create(userDataFilePath).Dispose();
            System.Diagnostics.Debug.WriteLine($"Created userData.csv file in: {FileSystem.Current.AppDataDirectory}");
        }


        foreach (string line in File.ReadLines(userDataFilePath))
        {
            if (String.IsNullOrWhiteSpace(line)) { return; }
            if (int.TryParse(line.Split(',')[0], out int id)) { userIds.Add(id); }
        }

        userIds.Sort();
        while (userIds.Contains(newUserIdNum))
        {
            newUserIdNum++;
        }

        UpdateUserElementsUI();
    }

    private void Login_BTN_Clicked(object sender, EventArgs e)
    {
        AccountLoginCheck(pass_ENT.Text);
    }

    private void NextUser_BTN_Clicked(object sender, EventArgs e)
    {
        if (userIdIndex < userIds.Count - 1) { userIdIndex++; }
        UpdateUserElementsUI();
    }

    private void PrevUser_BTN_Clicked(object sender, EventArgs e)
    {
        if (userIdIndex > -1) { userIdIndex--; }
        UpdateUserElementsUI();
    }

    private void Pass_ENT_Completed(object sender, EventArgs e)
    {
        AccountLoginCheck(pass_ENT.Text);
    }

    private void RemoveErrorLB(object sender, EventArgs e)
    {
        error_LB.Text = string.Empty;
    }

    private void UpdateUserElementsUI()
    {
        if (userIdIndex == -1)
        {
            pfp_IMG.Source = "blankuserui.png";
            user_ENT.IsVisible = true;
            user_LB.IsVisible = false;
        }
        else
        {
            userLoginParts = File.ReadLines(userDataFilePath).FirstOrDefault(line => line.StartsWith(userIds[userIdIndex].ToString())).Split(',');
            pfp_IMG.Source = Path.Combine(FileSystem.Current.AppDataDirectory, userLoginParts[0].ToString(), "pfp.png");
            user_ENT.IsVisible = false;
            user_LB.IsVisible = true;
            user_LB.Text = userLoginParts[1];
        }
    }

    private async void AccountLoginCheck(string pass)
    {
        if (userIdIndex != -1)
        {
            if (userLoginParts.Any(s => String.IsNullOrWhiteSpace(s))) { error_LB.Text = "Saved login data error"; return; }

            if (userLoginParts.Length == 3)
            {
                if (pass != userLoginParts[2]) { error_LB.Text = "Password is incorrect"; return; }

                MainPage.USER_ID = int.Parse(userLoginParts[0]);

                await Shell.Current.GoToAsync("..");
            }
        }
        else
        {
            if (String.IsNullOrWhiteSpace(user_ENT.Text) && String.IsNullOrWhiteSpace(pass_ENT.Text)) { error_LB.Text = "Username and Password can't be empty"; return; }
            if (String.IsNullOrWhiteSpace(user_ENT.Text)) { error_LB.Text = "Username can't be empty"; return; }
            if (String.IsNullOrWhiteSpace(pass_ENT.Text)) { error_LB.Text = "Password can't be empty"; return; }

            Directory.CreateDirectory(Path.Combine(FileSystem.Current.AppDataDirectory, $"{newUserIdNum}"));

            using Stream srcStream = await FileSystem.OpenAppPackageFileAsync(Path.Combine("Resources", "Raw", "blankuser.png"));
            using FileStream desStream = File.Create(Path.Combine(FileSystem.AppDataDirectory, newUserIdNum.ToString(), "pfp.png"));
            await srcStream.CopyToAsync(desStream);
            srcStream.Dispose();
            desStream.Dispose();

            File.AppendAllText(userDataFilePath, $"{newUserIdNum},{user_ENT.Text},{pass_ENT.Text}\n");

            MainPage.USER_ID = newUserIdNum;
            await Shell.Current.GoToAsync("..");
        }
    }
}