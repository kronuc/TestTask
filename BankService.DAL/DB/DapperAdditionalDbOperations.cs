using Dapper;

namespace BankService.DAL.DB
{
    public class DapperAdditionalDbOperations : IHostedService
    {
        private const string TABLE_NAME = "Transaction";
        private PostgresqlConnectionKeeper _keeper;

        public DapperAdditionalDbOperations(PostgresqlConnectionKeeper keeper)
        {
            _keeper = keeper;
        }

        public async Task CreateTableIfNotExists()
        {
            using var connection = await _keeper.GetConnection();
            var sql = $"CREATE TABLE IF NOT EXISTS public.{TABLE_NAME}" +
                $"(" +
                $"id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 )," +
                $"clienid bigint NOT NULL," +
                $"currency bigint NOT NULL," +
                $"amount double precision NOT NULL," +
                $"departmentaddress text COLLATE pg_catalog.\"default\" NOT NULL," +
                $"state text COLLATE pg_catalog.\"default\"," +
                $"CONSTRAINT \"{TABLE_NAME}_pkey\" PRIMARY KEY (id)" +
                $")";

            await connection.ExecuteAsync(sql);
        }

        public async Task AddOperations()
        {
            using var connection = await _keeper.GetConnection();
            var sql = $@"CREATE OR REPLACE FUNCTION public.get_by_id(
	                _id integer)
                    RETURNS text
                    LANGUAGE 'plpgsql'
                AS $BODY$
  	                DECLARE item_state text;
                begin
	                SELECT state INTO item_state FROM {TABLE_NAME} WHERE id = _id;
	                RETURN item_state;
                end;
                $BODY$;";

            await connection.ExecuteAsync(sql);
            sql = $@"CREATE OR REPLACE PROCEDURE public.create_transaction(
                    IN clientid integer,
                    IN departmentaddres text,
                    IN amount double precision,
                    IN currency integer,
                    IN state text)
                LANGUAGE 'plpgsql'
                AS $BODY$
                BEGIN
                    INSERT INTO {TABLE_NAME}(clienid, currency, amount, departmentaddress, state)
                    VALUES(clientid, currency, amount, departmentaddres, state);
                END;
                $BODY$;";

            await connection.ExecuteAsync(sql);
            sql = $@"CREATE OR REPLACE FUNCTION public.get_by_clientid(
	                _clientid integer,
	                _deparmantaddres text)
                RETURNS TABLE(ret text) 
                LANGUAGE 'plpgsql'
                AS $BODY$
                BEGIN
	                RETURN QUERY SELECT state FROM {TABLE_NAME} WHERE departmentaddress = _deparmantaddres and clienid = _clientid;
                END; 
                $BODY$;";

            await connection.ExecuteAsync(sql);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await CreateTableIfNotExists();
            await AddOperations();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
