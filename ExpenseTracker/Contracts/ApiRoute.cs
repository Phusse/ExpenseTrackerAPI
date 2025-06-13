namespace ExpenseTracker.Contracts;

public static class ApiRoute
{
    public static class Meta
    {
        public const string Base = "meta";
        public const string GetExpenseCategories = "expense-categories";
    }

    public static class Expenses
    {
        public const string Base = "expense";
        public const string Create = "";
        public const string GetById = "{id:guid}";
        public const string Filter = "filter";
        public const string GetAll = "getall";
        public const string Update = "{id:guid}";
        public const string Delete = "{id:guid}";
        public const string DeleteAll = "all";
    }
}
