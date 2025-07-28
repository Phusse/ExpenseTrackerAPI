namespace ExpenseTracker.Utilities.Routing;

/// <summary>
/// Defines all static API route paths used in the application.
/// Organizes them by domain (Expense, Budget, Savings, Dashboard),
/// and by HTTP verbs (Get, Post, Put, Patch, Delete).
/// </summary>
internal static class ApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";

    /// <summary>
    /// Routes related to Auth operations.
    /// </summary>
    public static class Auth
    {
        private const string Base = $"{Root}/{Version}/auth";

        /// <summary>POST endpoints for Auth.</summary>
        public static class Post
        {
            /// <summary>Logs in a user.</summary>
            public const string Login = $"{Base}/login";

            /// <summary>Registers a new user.</summary>
            public const string Register = $"{Base}/register";

            /// <summary>Logs out the user.</summary>
            public const string Logout = $"{Base}/logout";
        }

        /// <summary>GET endpoints for Auth.</summary>
        public static class Get
        {
            /// <summary>Gets the user</summary>
            public const string CurrentUser = $"{Base}/me";
        }
    }

    /// <summary>
    /// Routes related to Expense operations.
    /// </summary>
    public static class Expense
    {
        private const string Base = $"{Root}/{Version}/expense";

        /// <summary>POST endpoints for Expense.</summary>
        public static class Post
        {
            /// <summary>Creates a new expense entry.</summary>
            public const string Create = Base;
        }

        /// <summary>GET endpoints for Expense.</summary>
        public static class Get
        {
            /// <summary>Gets an expense by its ID.</summary>
            public const string ById = $"{Base}/{{id}}";

            /// <summary>Gets all expenses.</summary>
            public const string All = $"{Base}/getall";

            /// <summary>Filters expenses based on query params.</summary>
            public const string Filter = $"{Base}/filter";

            /// <summary>Gets the total sum of all expenses.</summary>
            public const string Total = $"{Base}/total";
        }

        /// <summary>PUT endpoints for Expense.</summary>
        public static class Put
        {
            /// <summary>Updates an expense by its ID.</summary>
            public const string Update = $"{Base}/{{id}}";
        }

        /// <summary>DELETE endpoints for Expense.</summary>
        public static class Delete
        {
            /// <summary>Deletes an expense by its ID.</summary>
            public const string ById = $"{Base}/{{id}}";

            /// <summary>Deletes all expenses.</summary>
            public const string All = $"{Base}/all";
        }
    }

    /// <summary>
    /// Routes related to Budget operations.
    /// </summary>
    public static class Budget
    {
        private const string Base = $"{Root}/{Version}/budget";

        /// <summary>POST endpoints for Budget.</summary>
        public static class Post
        {
            /// <summary>Creates a new budget entry.</summary>
            public const string Create = Base;
        }

        /// <summary>GET endpoints for Budget.</summary>
        public static class Get
        {
            /// <summary>Gets current budget status.</summary>
            public const string Status = $"{Base}/status";

            /// <summary>Gets a detailed overview of the user's budget.</summary>
            public const string Overview = $"{Base}/overview";
        }

        /// <summary>PUT endpoints for Budget.</summary>
        public static class Put
        {
            /// <summary>Updates the budget limit for a user.</summary>
            public const string Update = $"{Base}/update/{{id}}";
        }

        /// <summary>DELETE endpoints for Budget.</summary>
        public static class Delete
        {
            /// <summary>Delete the budget data</summary>
            public const string Remove = $"{Base}/delete/{{id}}";
        }
    }

    /// <summary>
    /// Routes related to Saving Goals.
    /// </summary>
    public static class Savings
    {
        private const string Base = $"{Root}/{Version}/savings";

        /// <summary>POST endpoints for Saving Goals.</summary>
        public static class Post
        {
            /// <summary>Creates a new savings goal.</summary>
            public const string Create = Base;

            /// <summary>Adds a contribution to a savings goal.</summary>
            public const string Contribute = $"{Base}/contribute";
        }

        /// <summary>GET endpoints for Saving Goals.</summary>
        public static class Get
        {
            /// <summary>Gets all savings goals.</summary>
            public const string All = $"{Base}/getall";

            /// <summary>Gets a savings goal by its ID.</summary>
            public const string ById = $"{Base}/{{id}}";
        }

        /// <summary>PUT endpoints for Saving Goals.</summary>
        public static class Put
        {
            /// <summary>Updates a savings goal by its ID.</summary>
            public const string Update = $"{Base}/{{id}}";
        }

        /// <summary>PATCH endpoints for Saving Goals.</summary>
        public static class Patch
        {
            /// <summary>Archives a savings goal by its ID.</summary>
            public const string Archive = $"{Base}/{{id}}/archive";
        }

        /// <summary>DELETE endpoints for Saving Goals.</summary>
        public static class Delete
        {
            /// <summary>Deletes a savings goal by its ID.</summary>
            public const string ById = $"{Base}/{{id}}";
        }
    }

    /// <summary>
    /// Routes related to Dashboard summary data.
    /// </summary>
    public static class Dashboard
    {
        private const string Base = $"{Root}/{Version}/dashboard";

        /// <summary>GET endpoints for Dashboard.</summary>
        public static class Get
        {
            /// <summary>Gets dashboard summary statistics.</summary>
            public const string Summary = $"{Base}/summary";
        }
    }

    /// <summary>
    /// Routes related to Metadata.
    /// </summary>
    public static class Metadata
    {
        private const string Base = $"{Root}/{Version}/metadata";

        public static class Get
        {
            /// <summary>Gets all expense categories.</summary>
            public const string ExpenseCategories = $"{Base}/expense-categories";

            /// <summary>Gets all payment methods.</summary>
            public const string PaymentMethods = $"{Base}/payment-methods";

            /// <summary>Gets all saving goal statuses.</summary>
            public const string SavingGoalStatuses = $"{Base}/saving-goal-statuses";
        }
    }
}
