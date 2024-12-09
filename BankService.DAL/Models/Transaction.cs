using NpgsqlTypes;
using SimpleBankSystem.API.Models;

namespace BankSystem.Server.Models
{
    public class Transaction
    {
        [PgName("id")]
        public int Id { get; set; }
        [PgName("clienid")]
        public int ClientId { get; set; }
        [PgName("departmentaddress")]
        public string DepartmentAddress { get; set; }
        [PgName("amount")]
        public double Amount { get; set; }
        [PgName("currency")]
        public Currency Currency { get; set; }
        [PgName("state")]
        public string State { get; set; }
    }
}
