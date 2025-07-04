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
        public const string Summary = $"{BudgetBase}/summary";
        public const string Status = $"{BudgetBase}/status";
    }
    //  savings Routes
    private const string SavingGoalController = "savings";
    public const string SavingGoalBase = $"{root}/{version}/{SavingGoalController}";

    public static class SavingGoalPostUrl
    {
        public const string Create = $"{SavingGoalBase}";                      // POST: api/v1/savings
        public const string Contribute = $"{SavingGoalBase}/contribute";       // POST: api/v1/savings/contribute
    }

    public static class SavingGoalGetUrl
    {
        public const string GetAll = $"{SavingGoalBase}/getall";               // GET: api/v1/savings/getall
        public const string GetById = $"{SavingGoalBase}/{{id}}";              // GET: api/v1/savings/{id}
    }

    public static class SavingGoalPutUrl
    {
        public const string Update = $"{SavingGoalBase}/{{id}}";               // PUT: api/v1/savings/{id}
    }

    public static class SavingGoalPatchUrl
    {
        public const string Archive = $"{SavingGoalBase}/{{id}}/archive";      // PATCH: api/v1/savings/{id}/archive
    }

    public static class SavingGoalDeleteUrl
    {
        public const string Delete = $"{SavingGoalBase}/{{id}}";               // DELETE: api/v1/savings/{id}
    }

    //  Dashboard Routes
    private const string DashboardController = "dashboard";
    public const string DashboardBase = $"{root}/{version}/{DashboardController}";

    public static class DashboardGetUrl
    {
        public const string Summary = $"{DashboardBase}/summary";
    }
}
