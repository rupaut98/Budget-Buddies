using Microsoft.Data.Sqlite;
using System;

namespace Budget_Buddies.Pages
{
    public partial class ExpenseLogPage : ContentPage
    {
        public ExpenseLogPage()
        {
            InitializeComponent();
        }

        private void OnExpenseEntered(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null && decimal.TryParse(entry.Text, out decimal expenseAmount))
            {
                
                string category = DetermineCategory(entry);
                if (!string.IsNullOrWhiteSpace(category))
                {
                    InsertExpenseIntoDatabase(category, expenseAmount);
                    entry.Text = ""; 
                }
            }
            else
            {
                // Handle invalid input
                DisplayAlert("Invalid Input", "Please enter a valid number for the expense.", "OK");
            }
        }

        private string DetermineCategory(Entry entry)
        {
           
            if (entry == FoodExpenseEntry)
                return "Food";
            else if (entry == UtilitiesExpenseEntry)
                return "Utilities";
            else if (entry == RentExpenseEntry)
                return "Rent";
            else if (entry == EntertainmentExpenseEntry)
                return "Entertainment";
            else
                return null; 
        }


        private void InsertExpenseIntoDatabase(string category, decimal amount)
        {
            App.DatabaseConnection.Open();

            var commandText = "INSERT INTO Expenses (Category, Amount) VALUES (@Category, @Amount);";
            using (var command = new SqliteCommand(commandText, App.DatabaseConnection))
            {
                command.Parameters.AddWithValue("@Category", category);
                command.Parameters.AddWithValue("@Amount", amount);
                command.ExecuteNonQuery();
            }

            App.DatabaseConnection.Close();

            LogExpensesFromDatabase();
        }

        private void LogExpensesFromDatabase()
        {
            App.DatabaseConnection.Open();

            var command = new SqliteCommand("SELECT * FROM Expenses", App.DatabaseConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["Id"]}, Category: {reader["Category"]}, Amount: {reader["Amount"]}");
            }

            App.DatabaseConnection.Close();
        }

    }
}
