using System.ComponentModel.DataAnnotations;
using SimpleBankSystem.API.Models;

namespace BankSystem.API.Models.Request
{
    public class CreateTransactionQuery
    {
        public required int ClientId { get; set; }

        public required string DepartmentAddress { get; set; }

        public required double Amount { get; set; }

        public required Currency Currency { get; set; }

    }
}
