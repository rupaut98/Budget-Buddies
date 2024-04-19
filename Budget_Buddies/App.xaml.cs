using Budget_Buddies.Pages;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Maui.Storage;

namespace Budget_Buddies;

public partial class App : Application
{
    public static SqliteConnection DatabaseConnection;

    public App()
    {

        InitializeComponent();

        InitializeDatabase();

        MainPage = new NavigationPage(new WelcomePage());
    }

    private void InitializeDatabase()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db3");
        DatabaseConnection = new SqliteConnection($"Filename={dbPath}");

        DatabaseConnection.Open();
        // Correctly formatted SQL command
        string tableCommand = @"
            CREATE TABLE IF NOT EXISTS Expenses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Category TEXT NOT NULL,
                Amount REAL NOT NULL
            );";

        var createTable = new SqliteCommand(tableCommand, DatabaseConnection);

        createTable.ExecuteNonQuery();

        DatabaseConnection.Close();
    }
}
