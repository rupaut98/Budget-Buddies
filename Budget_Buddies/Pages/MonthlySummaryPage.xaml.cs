using Microcharts;
using Microsoft.Data.Sqlite;
using SkiaSharp;
using System.Formats.Tar;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Budget_Buddies.Pages;

public partial class MonthlySummaryPage : ContentPage
{
    private string currencySymbol = "$";
    private string currencyPreference = "Dollars";

    float Food { get; set; }
    float Utilities { get; set; }
    float Rent { get; set; }
    float Entertainment { get; set; }
    decimal totalAmount { get; set; }

    public MonthlySummaryPage()
    {
        InitializeComponent();
        currencyPreference = SettingsPage.PreferencesHelper.GetCurrencyPreference();
        currencySymbol = currencyPreference == "Euros" ? "€" : "$";
        LoadSummary();

        ChartEntry[] entries = DisplayChart();

        chartView.Chart = new DonutChart
        {
            Entries = entries,
            IsAnimated = true,
        };
    }

    ChartEntry[] DisplayChart()
    {
        ChartEntry[] entries = new[]
        {
            new ChartEntry(Food)
            {
                Color = SKColor.Parse("#164D63")
            },
            new ChartEntry(Utilities)
            {
                Color = SKColor.Parse("#CAE6EC")
            },
            new ChartEntry(Rent)
            {
                Color = SKColor.Parse("#FF7171")
            },
            new ChartEntry(Entertainment)
            {
                Color = SKColor.Parse("#FAE6E6")
            },
        };
        return entries;
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

                totalAmount = totalAmount + totals[category];
            }
        }

        App.DatabaseConnection.Close();

        Food = float.Parse($"{totals["Food"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        Utilities = float.Parse($"{totals["Utilities"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        Rent = float.Parse($"{totals["Rent"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        Entertainment = float.Parse($"{totals["Entertainment"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        UpdateLabels(totals, mostExpensiveAmount, mostExpensiveCategory);
    }

    private decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        decimal conversionRate = fromCurrency == "USD" && toCurrency == "Euros" ? 0.92887359m : 1m;
        return Math.Round(amount * conversionRate, 2);
    }

    private void UpdateLabels(Dictionary<string, decimal> totals, decimal mostExpensiveAmount, string mostExpensiveCategory)
    {
        FoodTotalLabel.Text = $"{currencySymbol}{totals["Food"]}";
        UtilitiesTotalLabel.Text = $"{currencySymbol}{totals["Utilities"]}";
        RentTotalLabel.Text = $"{currencySymbol}{totals["Rent"]}";
        EntertainmentTotalLabel.Text = $"{currencySymbol}{totals["Entertainment"]}";
        Total.Text = $"{currencySymbol}{totalAmount.ToString()}";
    }

    private async void OnBackButtonClicked(object sender, EventArgs e) => await Navigation.PushAsync(new MenuPage());
}