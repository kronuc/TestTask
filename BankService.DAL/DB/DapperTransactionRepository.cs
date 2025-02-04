using BankService.DAL.Models;
using Dapper;

namespace BankService.DAL.DB
{

    public class DapperTransactionRepository
    {
        private const string TABLE_NAME = "Transaciontt2";
        private PostgresqlConnectionKeeper _keeper;
        public DapperTransactionRepository(PostgresqlConnectionKeeper keeper)
        {
            _keeper = keeper;

        }
        public async Task Add(Transaction game)
        {
            using var connection = await _keeper.GetConnection();
            var queryArguments = new
            {
                clientid = game.ClientId,
                currency = game.Currency,
                amount = game.Amount,
                departmentaddres = game.DepartmentAddress,
                state = "Pending"
            };
            await connection.ExecuteAsync("create_transaction", queryArguments, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<string> Get(int id)
        {
            using var connection = await _keeper.GetConnection();
            string commandText = $"SELECT * from get_by_id(@id)";

            var queryArgs = new { id };
            var game = await connection.QueryFirstAsync<string>(commandText, queryArgs);
            return game;
        }
        public async Task<IEnumerable<string>> GetByClient(int clientId, string address)
        {
            using var connection = await _keeper.GetConnection();
            string commandText = $"SELECT * from get_by_clientid(@id, @deparmantAddres)";

            var queryArgs = new { id = clientId, deparmantAddres = address };
            var game = await connection.QueryAsync<string>(commandText, queryArgs);
            return game;
        }

    }
}
