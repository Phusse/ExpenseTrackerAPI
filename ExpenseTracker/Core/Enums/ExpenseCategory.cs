using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Core.Enums;

public enum ExpenseCategory
{
	// Essentials
	[Display(Name = "Food")] Food,
	[Display(Name = "Groceries")] Groceries,
	[Display(Name = "Housing")] Housing,
	[Display(Name = "Utilities")] Utilities,
	[Display(Name = "Phone & Internet")] PhoneAndInternet,
	[Display(Name = "Transportation")] Transportation,
	[Display(Name = "Healthcare")] Healthcare,

	// Personal Expenses
	[Display(Name = "Personal Care")] PersonalCare,
	[Display(Name = "Entertainment")] Entertainment,
	[Display(Name = "Shopping")] Shopping,
	[Display(Name = "Travel")] Travel,

	// Financial
	[Display(Name = "Savings")] Savings,
	[Display(Name = "Investments")] Investments,
	[Display(Name = "Insurance")] Insurance,
	[Display(Name = "Term Loan Repayment")] TermLoanRepayment,
	[Display(Name = "Fees")] Fees,

	// Giving & Donations
	[Display(Name = "Donations")] Donations,
	[Display(Name = "Gifts")] Gifts,

	// Education & Others
	[Display(Name = "Education")] Education,
	[Display(Name = "Betting")] Betting,
	[Display(Name = "Miscellaneous")] Miscellaneous,
}
