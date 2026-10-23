using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace PotatoUnitedCreditUnion;

public partial class LoginPage : ContentPage
{
    private int userId = 0;
    private int maxUserId = -1;
    private string userLine = "";

    public LoginPage()
    {
        InitializeComponent();

        string filePath = Path.Combine(AppContext.BaseDirectory, "~APPDATA\\userData.csv");
        if (!File.Exists(filePath)) { System.Diagnostics.Debug.WriteLine($"File not found: {filePath}"); return; }

        maxUserId = File.ReadLines(filePath).Count() - 1;
        if (maxUserId <= -1) { System.Diagnostics.Debug.WriteLine($"User data file empty"); return; }

        userLine = File.ReadLines(filePath).ElementAtOrDefault(userId);
        pfp_IMG.Source = ImageSource.FromFile(Path.Combine(AppContext.BaseDirectory, $"~APPDATA\\{userId}\\pfp.png"));
    }

    private void Login_BTN_ClickedAsync(object sender, EventArgs e)
    {
        AccountLoginCheck(user_ENT.Text, pass_ENT.Text);
    }

    private void Pass_ENT_Completed(object sender, EventArgs e)
    {
        AccountLoginCheck(user_ENT.Text, pass_ENT.Text);
    }

    private async void AccountLoginCheck(string user, string pass)
    {
        if (String.IsNullOrWhiteSpace(userLine))
        {
            return;
        }
        if (userLine.Split(',') is string[] userLoginParts && userLoginParts.Length == 2)
        {
            if (user == userLoginParts[0] && pass == userLoginParts[1])
            {
                await Shell.Current.GoToAsync("..");
            }
        }
        return;
    }
}