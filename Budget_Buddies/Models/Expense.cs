using System;

namespace Budget_Buddies.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public decimal Amount { get; set; }
    }
}
