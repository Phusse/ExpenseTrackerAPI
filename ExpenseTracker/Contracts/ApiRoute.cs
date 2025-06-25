namespace ExpenseTracker.Contracts;

public static class ExpenseRoutes
{
    private const string root = "api";
    private const string version = "v1";

    // Expense Routes
    private const string expenseController = "expense";
    public const string ExpenseBase = $"{root}/{version}/{expenseController}";

    public static class PostUrl
    {
        public const string Create = $"{ExpenseBase}";
    }

    public static class GetUrl
    {
        public const string GetById = $"{ExpenseBase}/{{id}}";        // GET: api/v1/expense/{id}
        public const string GetAll = $"{ExpenseBase}/getall";         // GET: api/v1/expense/getall
        public const string Filter = $"{ExpenseBase}/filter";         // GET: api/v1/expense/filter
        public const string Total = $"{ExpenseBase}/total";           // GET: api/v1/expense/total
    }

    public static class PutUrl
    {
        public const string Update = $"{ExpenseBase}/{{id}}";         // PUT: api/v1/expense/{id}
    }

    public static class DeleteUrl
    {
        public const string Delete = $"{ExpenseBase}/{{id}}";         // DELETE: api/v1/expense/{id}
        public const string DeleteAll = $"{ExpenseBase}/all";         // DELETE: api/v1/expense/all
    }

    // ✅ Budget Routes
    private const string budgetController = "budget";
    public const string BudgetBase = $"{root}/{version}/{budgetController}";

    public static class BudgetPostUrl
    {
        public const string Create = $"{BudgetBase}";
    }

    public static class BudgetGetUrl
    {
        public const string Summary = $"{BudgetBase}/summary";        // GET: api/v1/budget/summary
        public const string Status = $"{BudgetBase}/status";          // GET: api/v1/budget/status
    }
}
