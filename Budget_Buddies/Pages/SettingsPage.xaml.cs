namespace Budget_Buddies.Pages;

public partial class SettingsPage : ContentPage
{
    string NameChange;
	public SettingsPage()
	{
		InitializeComponent();
	}

    private void OnChangeNameClicked(object sender, EventArgs e)
    {
        SettingsSubmit.IsVisible = false;
        ChangeName.IsVisible = true;
    }

    private void OnChangeSubmitNameClicked(object sender, EventArgs e)
    {
        NameChange = NameEntry.Text;
        ChangeName.IsVisible = false;
        SettingsSubmit.IsVisible = true;
    }
    private void OnChangeSettingsClicked(object sender, EventArgs e)
    {
        name.Text = NameChange;
    }
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MenuPage());
    }
}
