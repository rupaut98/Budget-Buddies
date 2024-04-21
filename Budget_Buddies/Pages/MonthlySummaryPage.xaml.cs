using Microsoft.Data.Sqlite;

namespace Budget_Buddies.Pages;

public partial class MonthlySummaryPage : ContentPage
{
    private string currencySymbol = "$"; // Default currency symbol
    private string currencyPreference = "Dollars";
    public MonthlySummaryPage()
    {
        InitializeComponent();
        currencyPreference = SettingsPage.PreferencesHelper.GetCurrencyPreference();

        if (currencyPreference == "Euros") // Corrected the syntax by using parentheses for the condition
        {
            currencySymbol = "€";
        }
        else
        {
            currencySymbol = "$";
        }
        LoadSummary();
    }

    private void LoadSummary()
    {
        decimal mostExpensiveAmount = 0;
        string mostExpensiveCategory = "";

        // Fetch exchange rate from an external source
        decimal exchangeRate = 0.94m; // Implement this method to fetch the exchange rate

        var totals = new Dictionary<string, decimal> { { "Food", 0 }, { "Utilities", 0 }, { "Rent", 0 }, { "Entertainment", 0 } };

        App.DatabaseConnection.Open();

        foreach (var category in totals.Keys.ToList())
        {
            var query = "SELECT SUM(Amount) FROM Expenses WHERE Category = @Category";
            using (var command = new SqliteCommand(query, App.DatabaseConnection))
            {
                command.Parameters.AddWithValue("@Category", category);
                var result = command.ExecuteScalar();

                decimal sum = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

                // Convert amount to euros if currencySymbol is €
                if (currencySymbol == "€")
                {
                    sum *= exchangeRate; // Convert to euros
                }

                totals[category] = sum;

                if (sum > mostExpensiveAmount)
                {
                    mostExpensiveAmount = sum;
                    mostExpensiveCategory = category;
                }
            }
        }

        App.DatabaseConnection.Close();


        FoodTotalLabel.Text = $"Amount for Food = {currencySymbol}{totals["Food"]}";
        UtilitiesTotalLabel.Text = $"Amount for Utilities = {currencySymbol}{totals["Utilities"]}";
        RentTotalLabel.Text = $"Amount for Rent = {currencySymbol}{totals["Rent"]}";
        EntertainmentTotalLabel.Text = $"Amount for Entertainment = {currencySymbol}{totals["Entertainment"]}";
        MostExpensiveLabel.Text = $"Most Expensive = {mostExpensiveCategory} with {currencySymbol}{mostExpensiveAmount}";
    }
}
