namespace ExpenseTracker.Contracts;

internal static class ApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";

    public static class Expense
    {
        private const string Base = $"{Root}/{Version}/expense";

        public static class Post
        {
            public const string Create = Base;
        }

        public static class Get
        {
            public const string ById = $"{Base}/{{id}}";
            public const string All = $"{Base}/getall";
            public const string Filter = $"{Base}/filter";
            public const string Total = $"{Base}/total";
        }

        public static class Put
        {
            public const string Update = $"{Base}/{{id}}";
        }

        public static class Delete
        {
            public const string ById = $"{Base}/{{id}}";
            public const string All = $"{Base}/all";
        }
    }

    public static class Budget
    {
        private const string Base = $"{Root}/{Version}/budget";

        public static class Post
        {
            public const string Create = Base;
        }

        public static class Get
        {
            public const string Summary = $"{Base}/summary";
            public const string Status = $"{Base}/status";
        }
    }

    public static class Savings
    {
        private const string Base = $"{Root}/{Version}/savings";

        public static class Post
        {
            public const string Create = Base;
            public const string Contribute = $"{Base}/contribute";
        }

        public static class Get
        {
            public const string All = $"{Base}/getall";
            public const string ById = $"{Base}/{{id}}";
        }

        public static class Put
        {
            public const string Update = $"{Base}/{{id}}";
        }

        public static class Patch
        {
            public const string Archive = $"{Base}/{{id}}/archive";
        }

        public static class Delete
        {
            public const string ById = $"{Base}/{{id}}";
        }
    }

    public static class Dashboard
    {
        private const string Base = $"{Root}/{Version}/dashboard";

        public static class Get
        {
            public const string Summary = $"{Base}/summary";
        }
    }
}
