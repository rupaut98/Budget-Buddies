namespace Budget_Buddies.Pages;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    private async void OnHomePageClicked(object sender, EventArgs e)
    {
    
        await Navigation.PushAsync(new HomePage());

    }

    private async void OnExpenseLogClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new ExpenseLogPage());

    }

    private async void OnMonthlySummaryClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new MonthlySummaryPage());

    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        
        await Navigation.PushAsync(new SettingsPage());

    }
}
