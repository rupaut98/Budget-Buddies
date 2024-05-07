using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace Budget_Buddies.Pages;

public partial class MonthlySummaryPage : ContentPage
{
    private string currencySymbol = "$";
    private string currencyPreference = "Dollars";

    public MonthlySummaryPage()
    {
        InitializeComponent();
        currencyPreference = SettingsPage.PreferencesHelper.GetCurrencyPreference();
        currencySymbol = currencyPreference == "Euros" ? "€" : "$";
        LoadSummary();
    }

    private void LoadSummary()
    {
        decimal mostExpensiveAmount = 0;
        string mostExpensiveCategory = "";

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

               
                sum = ConvertCurrency(sum, "USD", currencyPreference);

                totals[category] = sum;

                if (sum > mostExpensiveAmount)
                {
                    mostExpensiveAmount = sum;
                    mostExpensiveCategory = category;
                }
            }
        }

        App.DatabaseConnection.Close();

        UpdateLabels(totals, mostExpensiveAmount, mostExpensiveCategory);
    }

    private decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        decimal conversionRate = fromCurrency == "USD" && toCurrency == "Euros" ? 0.92887359m : 1m;
        return Math.Round(amount * conversionRate, 2);
    }

    private void UpdateLabels(Dictionary<string, decimal> totals, decimal mostExpensiveAmount, string mostExpensiveCategory)
    {
        FoodTotalLabel.Text = $"Amount for Food = {currencySymbol}{totals["Food"]}";
        UtilitiesTotalLabel.Text = $"Amount for Utilities = {currencySymbol}{totals["Utilities"]}";
        RentTotalLabel.Text = $"Amount for Rent = {currencySymbol}{totals["Rent"]}";
        EntertainmentTotalLabel.Text = $"Amount for Entertainment = {currencySymbol}{totals["Entertainment"]}";
        MostExpensiveLabel.Text = $"Most Expensive = {mostExpensiveCategory} with {currencySymbol}{mostExpensiveAmount}";
    }
}
