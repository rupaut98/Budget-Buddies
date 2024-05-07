using Android.Accounts;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Budget_Buddies.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            this.BindingContext = new HomePageViewModel();
        }
    }

    public class HomePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private decimal _budget;
        public decimal Budget
        {
            get => _budget;
            set
            {
                if (_budget != value)
                {
                    _budget = value;
                    OnPropertyChanged(nameof(Budget));
                    UpdateAmountLeft();
                }
            }
        }

        private string _currencySymbol = "$";
        public string CurrencySymbol
        {
            get => _currencySymbol;
            set
            {
                if (_currencySymbol != value)
                {
                    _currencySymbol = value;
                    OnPropertyChanged(nameof(CurrencySymbol));
                    // Redisplay all amounts in the new currency
                    LoadBudgetPreference();
                    LoadRecentExpenses();
                    UpdateAmountLeft();
                }
            }
        }

        private decimal _amountLeft;
        public decimal AmountLeft
        {
            get => _amountLeft;
            set
            {
                if (_amountLeft != value)
                {
                    _amountLeft = value;
                    OnPropertyChanged(nameof(AmountLeft));
                }
            }
        }

        public ObservableCollection<Expense> RecentExpenses { get; set; } = new ObservableCollection<Expense>();

        public HomePageViewModel()
        {
            LoadCurrencyPreference();
            LoadBudgetPreference();
            LoadRecentExpenses();
        }

        private decimal ConvertAmountToCurrentCurrency(decimal amountInDollars)
        {
            decimal conversionRate = CurrencySymbol == "€" ? GetUsdToEuroRate() : 1m;
            return Math.Round(amountInDollars * conversionRate, 2);
        }

        private void LoadCurrencyPreference()
        {
            string preference = SettingsPage.PreferencesHelper.GetCurrencyPreference();
            CurrencySymbol = preference == "Dollars" ? "$" : "€";
        }

        private decimal GetUsdToEuroRate()
        {
            return 0.92887359m; 
        }

        private void LoadBudgetPreference()
        {
            using (var connection = new SqliteConnection($"Filename={App.DatabasePath}"))
            {
                connection.Open();
                var commandText = "SELECT Budget FROM Preferences WHERE Id = 0;";
                using (var command = new SqliteCommand(commandText, connection))
                {
                    var result = command.ExecuteScalar();
                    _budget = result != null ? Convert.ToDecimal(result) : 0;
                }
                connection.Close();
            }
            Budget = ConvertAmountToCurrentCurrency(_budget);
        }

        private void LoadRecentExpenses()
        {
            RecentExpenses.Clear();
            foreach (var expense in GetRecentExpensesFromDatabase())
            {
                RecentExpenses.Add(new Expense
                {
                    ExpenseName = expense.ExpenseName,
                    ExpenseAmount = ConvertAmountToCurrentCurrency(expense.ExpenseAmount)
                });
            }
            UpdateAmountLeft();
        }

        private List<Expense> GetRecentExpensesFromDatabase()
        {
            var expenses = new List<Expense>();
            using (var connection = new SqliteConnection($"Filename={App.DatabasePath}"))
            {
                connection.Open();
                var commandText = "SELECT Category, Amount FROM Expenses ORDER BY Id DESC LIMIT 5";
                using (var command = new SqliteCommand(commandText, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expenses.Add(new Expense
                            {
                                ExpenseName = reader["Category"].ToString(),
                                ExpenseAmount = Convert.ToDecimal(reader["Amount"])
                            });
                        }
                    }
                }
                connection.Close();
            }
            return expenses;
        }

        private void UpdateAmountLeft()
        {
            decimal totalExpenses = GetAllExpensesSumFromDatabase();
            AmountLeft = Budget - totalExpenses; 
            AmountLeft = ConvertAmountToCurrentCurrency(AmountLeft); 
        }

        private decimal GetAllExpensesSumFromDatabase()
        {
            decimal totalExpenses = 0;
            using (var connection = new SqliteConnection($"Filename={App.DatabasePath}"))
            {
                connection.Open();
                var commandText = "SELECT SUM(Amount) FROM Expenses";
                using (var command = new SqliteCommand(commandText, connection))
                {
                    var result = command.ExecuteScalar();
                    totalExpenses = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                }
                connection.Close();
            }
            return totalExpenses;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Expense
    {
        public string ExpenseName { get; set; }
        public decimal ExpenseAmount { get; set; }
    }
}