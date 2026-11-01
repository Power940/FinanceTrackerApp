using System.Formats.Asn1;

namespace PotatoUnitedCreditUnion
{
    public partial class MainPage : ContentPage
    {
        public static int USER_ID { get; set; } = -1;
        public static string USER_DIR_PATH { get; set; } = "";

        public MainPage()
        {
            InitializeComponent();
            Shell.Current.GoToAsync(nameof(LoginPage));
        }

        private void Login_BTN_Clicked(object? sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(LoginPage));
        }

        private void Daily_BTN_Clicked(object? sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(DailyTransactionPage));
        }

        private void History_BTN_Clicked(object? sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(TransactionHistoryPage));
        }
    }
}
