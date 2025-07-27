using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Enums;

/// <summary>
/// Represents the method used for a payment transaction.
/// </summary>
public enum PaymentMethod
{
	/// <summary>
	/// Payment made with physical cash.
	/// </summary>
	[Display(Name = "Cash")]
	Cash,

	/// <summary>
	/// Payment made using a credit or debit card.
	/// </summary>
	[Display(Name = "Card")]
	Card,

	/// <summary>
	/// Payment made via bank transfer.
	/// </summary>
	[Display(Name = "Bank Transfer")]
	BankTransfer,

	/// <summary>
	/// Payment made using a mobile wallet or app (e.g., Apple Pay, Google Pay).
	/// </summary>
	[Display(Name = "Mobile Payment")]
	MobilePayment,

	/// <summary>
	/// Payment made using a POS (Point of Sale) device.
	/// </summary>
	[Display(Name = "POS")]
	POS,

	/// <summary>
	/// Payment made through an online payment gateway (e.g., PayPal, Stripe).
	/// </summary>
	[Display(Name = "Online Gateway")]
	OnlineGateway,

	/// <summary>
	/// Other or uncategorized payment methods.
	/// </summary>
	[Display(Name = "Other")]
	Other,
}
