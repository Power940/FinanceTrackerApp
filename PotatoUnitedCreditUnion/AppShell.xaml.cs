namespace PotatoUnitedCreditUnion
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(NewPage1), typeof(NewPage1));

            // Register page:       Routing.RegisterRoute(nameof(PageClass), typeof(PageClass));
            // move to:             await Shell.Current.GoToAsync(nameof(PageClass));
            // go back:             await Shell.Current.GoToAsync("..");
        }
    }
}
