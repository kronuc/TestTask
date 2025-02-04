using BankService.DAL.DB;

namespace BankService.DAL.DB
{
    public static class AddDbConnectionExtension
    {
        public static void AddDbConnection(this IServiceCollection services)
        {
            services.AddSingleton<PostgresqlConnectionKeeper>();
            services.AddHostedService<DapperAdditionalDbOperations>();
            services.AddSingleton<DapperTransactionRepository>();
        }
    }
}
