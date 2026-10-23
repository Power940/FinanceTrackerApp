namespace PotatoUnitedCreditUnion
{
    public partial class AppShell : Shell
    {
        public int USER_ID { get; set; } = -1;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

            // how to do stuff with multiple pages
            // Register page:       Routing.RegisterRoute(nameof(PageClass), typeof(PageClass));
            // move to:             await Shell.Current.GoToAsync(nameof(PageClass));
            // go back:             await Shell.Current.GoToAsync("..");
        }
    }
}
