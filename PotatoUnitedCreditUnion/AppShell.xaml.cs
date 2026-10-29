namespace PotatoUnitedCreditUnion
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(DailyTransactionPage), typeof(DailyTransactionPage));
            Routing.RegisterRoute(nameof(TransactionHistoryPage), typeof(TransactionHistoryPage));

            // how to do stuff with multiple pages
            // Register page:       Routing.RegisterRoute(nameof(PageClass), typeof(PageClass));
            // move to:             await Shell.Current.GoToAsync(nameof(PageClass));
            // go back:             await Shell.Current.GoToAsync("..");
        }
    }
}
