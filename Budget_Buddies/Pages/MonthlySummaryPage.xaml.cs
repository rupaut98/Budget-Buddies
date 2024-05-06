using Microcharts;
using Microsoft.Data.Sqlite;
using SkiaSharp;
using System.Formats.Tar;
using System.Globalization;
namespace Budget_Buddies.Pages;

public partial class MonthlySummaryPage : ContentPage
{
    private string currencySymbol = "$";
    private string currencyPreference = "Dollars";
    float Food {  get; set; }
    float Utilities { get; set; }
    float Rent {  get; set; }
    float Entertainment { get; set; }

    decimal totalAmount { get; set; }
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

        ChartEntry[] entries = DisplayChart();

        chartView.Chart = new DonutChart
        {
            Entries = entries,
            IsAnimated = true,
        };

        DisplayAmount();
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

        // Fetch exchange rate from an external source
        decimal exchangeRate = 0.94m; // Implement this method to fetch the exchange rate

        var totals = new Dictionary<string, decimal>{{ "Food", 0 },{ "Utilities", 0 },{ "Rent", 0 },{ "Entertainment", 0 }};

        
        App.DatabaseConnection.Open();

        foreach (var category in totals.Keys.ToList())
        {
            var query = "SELECT SUM(Amount) FROM Expenses WHERE Category = @Category";
            using (var command = new SqliteCommand(query, App.DatabaseConnection))
            {
                command.Parameters.AddWithValue("@Category", category);
                var result = command.ExecuteScalar();

                
                decimal sum = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                totals[category] = sum;

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

                totalAmount = totalAmount + totals[category];
            }
        }

        
        App.DatabaseConnection.Close();

        Food = float.Parse($"{totals["Food"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        Utilities = float.Parse($"{totals["Utilities"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        Rent = float.Parse($"{totals["Rent"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

        Entertainment = float.Parse($"{totals["Entertainment"]}", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
       // MostExpensiveLabel.Text = $"Most Expensive = {mostExpensiveCategory} with ${mostExpensiveAmount}";

    }

    void DisplayAmount()
    {
        FoodTotalLabel.Text = $"${Food.ToString()}";
        
        UtilitiesTotalLabel.Text = $"${Utilities.ToString()}";

        RentTotalLabel.Text = $"${Rent.ToString()}";

        EntertainmentTotalLabel.Text = $"${Entertainment.ToString()}";

        Total.Text = $"${totalAmount.ToString()}";
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MenuPage());
    }

  
}
