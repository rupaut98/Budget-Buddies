namespace Budget_Buddies.Pages;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnGuestSignInClicked(object sender, EventArgs e)
    { 
        await Navigation.PushAsync(new MenuPage());
    }
}
