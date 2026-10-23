using System.Formats.Asn1;

namespace PotatoUnitedCreditUnion
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}
