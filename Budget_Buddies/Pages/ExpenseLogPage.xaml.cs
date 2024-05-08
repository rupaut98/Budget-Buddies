using Microsoft.Data.Sqlite;
using System;
using System.ComponentModel;

namespace Budget_Buddies.Pages
{
    public partial class ExpenseLogPage : ContentPage
    {
        private string currencySymbol;
        public string CurrencySymbol
        {
            get => currencySymbol;
            set
            {
                if (currencySymbol != value)
                {
                    currencySymbol = value;
                    OnPropertyChanged(nameof(CurrencySymbol));
                }
            }
        }

        public ExpenseLogPage()
        {
            InitializeComponent();
            LoadCurrencyPreference();
            BindingContext = this;
        }

        private void LoadCurrencyPreference()
        {
            string preference = SettingsPage.PreferencesHelper.GetCurrencyPreference();
            CurrencySymbol = preference == "Dollars" ? "$" : "€";
        }

        private void OnExpenseEntered(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null && decimal.TryParse(entry.Text, out decimal expenseAmount))
            {
                string category = DetermineCategory(entry);
                if (!string.IsNullOrWhiteSpace(category))
                {
                    
                    expenseAmount = ConvertToUSDIfNeeded(expenseAmount);
                    InsertExpenseIntoDatabase(category, expenseAmount);
                    entry.Text = "";
                }
            }
            else
            {
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
                command.Parameters.AddWithValue("@Amount", Math.Round(amount, 2));
                command.ExecuteNonQuery();
            }
            App.DatabaseConnection.Close();
            LogExpensesFromDatabase();
        }

        
        private decimal ConvertToUSDIfNeeded(decimal amount)
        {
            if (CurrencySymbol == "€")
            {
                decimal conversionRate = GetEuroToUsdRate();
                return Math.Round(amount * conversionRate, 2);
            }
            return amount; 
        }

        private decimal GetEuroToUsdRate()
        {
            
            return 1.0765523m;
        }

        new public event PropertyChangedEventHandler PropertyChanged;

        override protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LogExpensesFromDatabase()
        {
            App.DatabaseConnection.Open();
            var commandText = "SELECT * FROM Expenses ORDER BY Id DESC LIMIT 5";
            using (var command = new SqliteCommand(commandText, App.DatabaseConnection))
            {
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["Id"]}, Category: {reader["Category"]}, Amount: {reader["Amount"]}");
                }
            }
            App.DatabaseConnection.Close();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuPage());
        }
    }
}