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

        //ClearDatabaseData();
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

        DatabaseConnection.Close();
    }

    public void ClearDatabaseData()
    {
        DatabaseConnection.Open();

        var clearExpensesTable = new SqliteCommand("DELETE FROM Expenses;", DatabaseConnection);
        clearExpensesTable.ExecuteNonQuery();

        var clearPreferencesTable = new SqliteCommand("DELETE FROM Preferences;", DatabaseConnection);
        clearPreferencesTable.ExecuteNonQuery();

        DatabaseConnection.Close();
    }
}
