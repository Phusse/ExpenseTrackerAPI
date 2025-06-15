namespace ExpenseTracker.Contracts;

public static class ExpenseRoutes
{
    private const string root = "api";
    private const string version = "v1";
    private const string controller = "expense";
    public const string Base = $"{root}/{version}/{controller}";

    public static class PostUrl
    {
        public const string Create = $"{Base}";
    }

    public static class GetUrl
    {
        public const string GetById = $"{Base}/{{id}}";        // GET: api/v1/expense/{id}
        public const string GetAll = $"{Base}/getall";         // GET: api/v1/expense/getall
        public const string Filter = $"{Base}/filter";         // GET: api/v1/expense/filter
    }
    public static class PutUrl
    {
        public const string Update = $"{Base}/{{id}}";         // PUT: api/v1/expense/{id}
    }
    public static class DeleteUrl
    {
        public const string Delete = $"{Base}/{{id}}";         // DELETE: api/v1/expense/{id}
        public const string DeleteAll = $"{Base}/all";         // DELETE: api/v1/expense/all
    }
}