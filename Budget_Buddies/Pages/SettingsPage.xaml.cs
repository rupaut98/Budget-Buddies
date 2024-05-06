using Microsoft.Data.Sqlite;

namespace Budget_Buddies.Pages;

public partial class SettingsPage : ContentPage
{
    RadioButton dollarsRadioButton;
    RadioButton eurosRadioButton;
    string NameChange;
	public SettingsPage()
	{
		InitializeComponent();
        LoadBudgetPreference();
        LoadCurrencyPreference();
    }

    /*private void OnChangeNameClicked(object sender, EventArgs e)
    {
        ChangeName.IsVisible = true;
    }

    private void OnChangeSubmitNameClicked(object sender, EventArgs e)
    {
        NameChange = NameEntry.Text;
        ChangeName.IsVisible = false;
    }*/
    private void OnSaveButtonClicked(object sender, EventArgs e)
    {

        if (decimal.TryParse(BudgetPreference.Text, out decimal budget))
        {
            SaveBudgetPreference(budget);
            DisplayAlert("Success", "Budget preference saved.", "OK");
        }
        else
        {
            DisplayAlert("Error", "Invalid budget entry. Please enter a number.", "OK");
        }
    }
   /* private void OnChangeSettingsClicked(object sender, EventArgs e)
    {
        name.Text = NameChange;
    }*/
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MenuPage());
    }

    private void LoadBudgetPreference()
    {

        BudgetPreference.Text = GetBudgetPreference().ToString("F2");
    }
    private decimal GetBudgetPreference()
    {
        using (var connection = new SqliteConnection($"Filename={App.DatabasePath}"))
        {
            connection.Open();
            var commandText = "SELECT Budget FROM Preferences WHERE Id = 0;";
            using (var command = new SqliteCommand(commandText, connection))
            {
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToDecimal(result);
                }
            }
            return 0;
        }
    }
    private void SaveBudgetPreference(decimal budget)
    {
        using (var connection = new SqliteConnection($"Filename={App.DatabasePath}"))
        {
            connection.Open();
            var commandText = @"
                INSERT INTO Preferences (Id, Budget) VALUES (0, @Budget)
                ON CONFLICT(Id) DO UPDATE SET Budget = @Budget;";
            using (var command = new SqliteCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Budget", budget);
                command.ExecuteNonQuery();
            }
        }
    }
    public static class PreferencesHelper
    {
        public static string GetCurrencyPreference()
        {
            using (var connection = new SqliteConnection($"Filename={App.DatabasePath}"))
            {
                connection.Open();
                var commandText = "SELECT Currency FROM Preferences WHERE Id = 0;";
                using (var command = new SqliteCommand(commandText, connection))
                {
                    var result = command.ExecuteScalar();
                    return result?.ToString() ?? "Dollars";
                }
            }
        }
    }

    private void SaveCurrencyPreference(string currency)
    {
        using (var connection = new SqliteConnection($"Filename={App.DatabasePath}"))
        {
            connection.Open();
            var commandText = @"
            UPDATE Preferences
            SET Currency = @Currency
            WHERE Id = 0;
            INSERT INTO Preferences (Id, Budget, Currency)
            SELECT 0, 0, @Currency
            WHERE (SELECT Changes() = 0);";
            using (var command = new SqliteCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Currency", currency);
                command.ExecuteNonQuery();
            }
        }
    }

    private void LoadCurrencyPreference()
    {
        string currency = SettingsPage.PreferencesHelper.GetCurrencyPreference();

        dollarsRadioButton = this.FindByName<RadioButton>("DollarsRadioButton");
        eurosRadioButton = this.FindByName<RadioButton>("EurosRadioButton");

        if (currency == "Dollars")
        {
            dollarsRadioButton.IsChecked = true;
        }
        else if (currency == "Euros")
        {
            eurosRadioButton.IsChecked = true;
        }


        dollarsRadioButton.CheckedChanged += OnCurrencyPreferenceChanged;
        eurosRadioButton.CheckedChanged += OnCurrencyPreferenceChanged;
    }

    private void OnCurrencyPreferenceChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var radioButton = (RadioButton)sender;
            SaveCurrencyPreference(radioButton.Content.ToString().Split(' ')[1]);

        }
    }
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WelcomePage());
    }
}
