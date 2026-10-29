using System.Formats.Asn1;

namespace PotatoUnitedCreditUnion
{
    public partial class MainPage : ContentPage
    {
        public static int USER_ID { get; set; } = -1;

        public MainPage()
        {
            InitializeComponent();
            Shell.Current.GoToAsync(nameof(LoginPage));
        }

        private void login_BTN_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}
