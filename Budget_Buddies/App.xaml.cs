using Budget_Buddies.Pages;
using Microsoft.Data.Sqlite;
using System.IO;
using Microsoft.Maui.Storage;

namespace Budget_Buddies;

public partial class App : Application
{
    public static string DatabasePath { get; private set; }
    public static SqliteConnection DatabaseConnection { get; private set; }

    public App()
    {
        InitializeComponent();

        InitializeDatabase();

        MainPage = new NavigationPage(new WelcomePage());
    }

    private void InitializeDatabase()
    {
        DatabasePath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db3");
        DatabaseConnection = new SqliteConnection($"Filename={DatabasePath}");

        DatabaseConnection.Open();

        var expensesTableCommand = @"
    CREATE TABLE IF NOT EXISTS Expenses (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Category TEXT NOT NULL,
        Amount REAL NOT NULL
    );";

        var createExpensesTable = new SqliteCommand(expensesTableCommand, DatabaseConnection);
        createExpensesTable.ExecuteNonQuery();

        
        var preferencesTableCommand = @"
    CREATE TABLE IF NOT EXISTS Preferences (
        Id INTEGER PRIMARY KEY CHECK (Id = 0),
        Budget REAL NOT NULL,
        Currency TEXT NOT NULL DEFAULT 'Dollars'
    );";

        var createPreferencesTable = new SqliteCommand(preferencesTableCommand, DatabaseConnection);
        createPreferencesTable.ExecuteNonQuery();


        var modifyPreferencesTableToAddCurrency = @"
    PRAGMA table_info(Preferences);
    ";

        var checkTableCommand = new SqliteCommand(modifyPreferencesTableToAddCurrency, DatabaseConnection);
        var reader = checkTableCommand.ExecuteReader();
        bool currencyColumnExists = false;

        while (reader.Read())
        {
            if (reader.GetString(1) == "Currency")
            {
                currencyColumnExists = true;
                break;
            }
        }

        if (!currencyColumnExists)
        {
            var alterTableCommandText = @"
        ALTER TABLE Preferences ADD COLUMN Currency TEXT DEFAULT 'Dollars';
        ";
            var alterTableCommand = new SqliteCommand(alterTableCommandText, DatabaseConnection);
            alterTableCommand.ExecuteNonQuery();
        }

        DatabaseConnection.Close();
    }
}