using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Enums;

/// <summary>
/// Represents the predefined categories used to classify user expenses.
/// </summary>
public enum ExpenseCategory
{
    /// <summary>
    /// Expenses related to food and groceries.
    /// </summary>
    [Display(Name = "Food & Groceries")]
    Food,

    /// <summary>
    /// Expenses related to transportation, such as fuel, public transit, or ride-hailing services.
    /// </summary>
    [Display(Name = "Transport")]
    Transport,

    /// <summary>
    /// Expenses for healthcare services, medications, and medical supplies.
    /// </summary>
    [Display(Name = "Health & Medical")]
    Health,

    /// <summary>
    /// Expenses for entertainment, such as movies, games, events, or subscriptions.
    /// </summary>
    [Display(Name = "Entertainment")]
    Entertainment,

    /// <summary>
    /// Expenses for utility services like electricity, water, internet, and phone bills.
    /// </summary>
    [Display(Name = "Utilities")]
    Utilities,

    /// <summary>
    /// Expenses for educational needs such as tuition, books, and online courses.
    /// </summary>
    [Display(Name = "Education")]
    Education,

    /// <summary>
    /// Money set aside as personal savings.
    /// </summary>
    [Display(Name = "Savings")]
    Savings,

    /// <summary>
    /// Money allocated to investments such as stocks, mutual funds, or crypto.
    /// </summary>
    [Display(Name = "Investments")]
    Investments,

    /// <summary>
    /// Expenses that do not fit into any of the defined categories.
    /// </summary>
    [Display(Name = "Miscellaneous")]
    Miscellaneous,
}
