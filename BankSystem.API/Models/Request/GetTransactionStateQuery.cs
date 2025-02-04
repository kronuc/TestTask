using System.ComponentModel.DataAnnotations;

namespace BankSystem.API.Models.Request
{
    public class GetTransactionStateQuery
    {
        public required int ClientId { get; set; }

        public required string DepartmentAddress { get; set; }
    }
}
