using Microsoft.Data.Sqlite;

namespace Budget_Buddies.Pages;

public partial class MonthlySummaryPage : ContentPage
{
	public MonthlySummaryPage()
	{
		InitializeComponent();
        LoadSummary();
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

                
                if (sum > mostExpensiveAmount)
                {
                    mostExpensiveAmount = sum;
                    mostExpensiveCategory = category;
                }
            }
        }

        
        App.DatabaseConnection.Close();

        FoodTotalLabel.Text = $"Amount for Food = ${totals["Food"]}";
        UtilitiesTotalLabel.Text = $"Amount for Utilities = ${totals["Utilities"]}";
        RentTotalLabel.Text = $"Amount for Rent = ${totals["Rent"]}";
        EntertainmentTotalLabel.Text = $"Amount for Entertainment = ${totals["Entertainment"]}";
        MostExpensiveLabel.Text = $"Most Expensive = {mostExpensiveCategory} with ${mostExpensiveAmount}";
    }

}
