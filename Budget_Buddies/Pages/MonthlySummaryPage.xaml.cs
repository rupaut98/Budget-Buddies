using Microcharts;
using Microsoft.Data.Sqlite;
using SkiaSharp;
using System.Formats.Tar;
using System.Globalization;
namespace Budget_Buddies.Pages;

public partial class MonthlySummaryPage : ContentPage
{
    float Food {  get; set; }
    float Utilities { get; set; }
    float Rent {  get; set; }
    float Entertainment { get; set; }

    float totalAmount { get; set; }
	public MonthlySummaryPage()
	{
		InitializeComponent();
        LoadSummary();

        ChartEntry[] entries = DisplayChart();

        chartView.Chart = new PieChart()
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
                Label = "Food",
                ValueLabel= Food.ToString(),
                Color = SKColor.Parse("#164D63")
            },
            new ChartEntry(Utilities)
            {
                Label = "Utilities",
                ValueLabel = Utilities.ToString(),
                Color = SKColor.Parse("#FF7171")
            },
            new ChartEntry(Rent)
            {
                Label = "Rent",
                ValueLabel= Rent.ToString(),
                Color = SKColor.Parse("#CAE6EC")
            },
            new ChartEntry(Entertainment)
            {
                Label = "Entertainment",
                ValueLabel= Entertainment.ToString(),
                Color = SKColor.Parse("#FAE6E6")
            },
        };
        return entries;
    }

    private void LoadSummary()
    {
        decimal mostExpensiveAmount = 0;
        string mostExpensiveCategory = "";

        
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
                string placeholder = sum.ToString();

                totalAmount = totalAmount + Int32.Parse(placeholder);
                
                if (sum > mostExpensiveAmount)
                {
                    mostExpensiveAmount = sum;
                    mostExpensiveCategory = category;
                }
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
