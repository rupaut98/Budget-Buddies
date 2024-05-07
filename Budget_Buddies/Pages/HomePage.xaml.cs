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
                    UpdateAmountLeft();
                    UpdateExpensesCurrency();
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

        private void UpdateExpensesCurrency()
        {

            var updatedExpenses = RecentExpenses.Select(expense => new Expense
            {
                ExpenseName = expense.ExpenseName,
                ExpenseAmount = ConvertAmountToCurrentCurrency(expense.ExpenseAmount)
            }).ToList();

            RecentExpenses.Clear();
            foreach (var exp in updatedExpenses)
            {
                RecentExpenses.Add(exp);
            }


            OnPropertyChanged(nameof(RecentExpenses));
        }

        private decimal ConvertAmountToCurrentCurrency(decimal amountInDollars)
        {
            return amountInDollars * (CurrencySymbol == "€" ? GetUsdToEuroRate() : 1m);
        }

        private void LoadCurrencyPreference()
        {
            string preference = SettingsPage.PreferencesHelper.GetCurrencyPreference();
            CurrencySymbol = preference == "Dollars" ? "$" : "€";
        }

        private decimal GetUsdToEuroRate()
        {
            return 0.94m;
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
                    Budget = result != null ? Convert.ToDecimal(result) : 0;
                    if (CurrencySymbol == "€")
                    {
                        Budget *= GetUsdToEuroRate();
                    }
                }
                connection.Close();
            }
        }

        private void LoadRecentExpenses()
        {
            RecentExpenses.Clear();
            foreach (var expense in GetRecentExpensesFromDatabase())
            {
                RecentExpenses.Add(new Expense
                {
                    ExpenseName = expense.ExpenseName,
                    ExpenseAmount = expense.ExpenseAmount * (CurrencySymbol == "€" ? GetUsdToEuroRate() : 1)
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
            OnPropertyChanged(nameof(AmountLeft));
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
            totalExpenses *= (CurrencySymbol == "€" ? GetUsdToEuroRate() : 1);
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
