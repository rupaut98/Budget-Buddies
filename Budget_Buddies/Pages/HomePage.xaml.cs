namespace Budget_Buddies.Pages;

public partial class HomePage : ContentPage
{
    decimal Budget {  get; set; }
    decimal Left { get; set; }

    Dictionary<string, decimal> expenseTracker { get; set; }

	public HomePage()
	{
		InitializeComponent();
	}

    private void CalculateLeft()
    {
        Left = Budget;

        Dictionary<string, decimal>.ValueCollection recentExpenseTotal = expenseTracker.Values;

        foreach (decimal expense in recentExpenseTotal)
        {
            Left = Left - expense;
        }

        LeftAmount.Text = Left.ToString();
    }

	private void OnBudgetEntered(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        if (entry != null && decimal.TryParse(entry.Text, out decimal budgetAmount))
        {

            Budget = budgetAmount;
            CalculateLeft();
        }
        else
        {
            // Handle invalid input
            DisplayAlert("Invalid Input", "Please enter a valid number for the budget.", "OK");
        }
    }

    private void OnRecentExpenseEntered(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        if (entry != null && decimal.TryParse(entry.Text, out decimal expenseAmount))
        {
            
            if(ExpenseName.hasPlaceholder())
            {
                ExpenseName.Text = "Placeholder";
                ExpenseAmount.Text = expenseAmount;
                expenseTracker.Add(ExpenseName.Text, expenseAmount);
            }
            else
            {
                ExpenseAmount.Text = expenseAmount;
                expenseTracker[ExpenseName.Text] = expenseAmount;
            }
            CalculateLeft();
        }
        else
        {
            // Handle invalid input
            DisplayAlert("Invalid Input", "Please enter a valid number for the expense.", "OK");
        }

        private void OnRecentExpenseNameEntered(object sender, EventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null)
            {

                if (ExpenseAmount.hasPlaceholder())
                {
                    decimal expenseAmount = 0;
                    ExpenseAmount.Text = expenseAmount.ToString();
                    expenseTracker.Add(ExpenseName.Text, expenseAmount);
                }
                else
                {
                    decimal expenseAmount = expenseTracker["Placeholder"].Value;
                    expenseTracker.Add(ExpenseName.Text, expenseAmount);
                    expenseTracker.Remove("Placeholder");
                }
                CalculateLeft();
            }
            else
            {
                // Handle invalid input
                DisplayAlert("Invalid Input", "Please enter a valid name for the expense.", "OK");
            }
        }
}
