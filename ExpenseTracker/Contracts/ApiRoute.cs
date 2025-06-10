namespace ExpenseTracker.Contracts;

public static class ApiRoute
{
    public static class Transactions
    {
        public const string Base = "api/transactions";
        public const string Create = $"{Base}";
        public const string GetById = $"{Base}/{{id}}";
        public const string GetAll = $"{Base}/all";
    }
}
