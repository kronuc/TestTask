using Npgsql;

namespace BankService.DAL.DB
{
    public class PostgresqlConnectionKeeper
    {
        private const string DEFAULT_CONNECTION_STRING = "Host=bank_system_db:5432;" +
                    "Username=user-name;" +
                    "Password=strong-password;" +
                    "Database=TransactionDB";
        private string _connectionString;

        public PostgresqlConnectionKeeper()
        {
            _connectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING") ?? DEFAULT_CONNECTION_STRING;

        }
        public async Task<NpgsqlConnection> GetConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            while (connection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception)
                {
                }
                await Task.Delay(1000);
            }
            return connection;
        }
    }
}
