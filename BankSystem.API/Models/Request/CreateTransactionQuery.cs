using System.ComponentModel.DataAnnotations;
using SimpleBankSystem.API.Models;

namespace BankSystem.API.Models.Request
{
    public class CreateTransactionQuery
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public string DepartmentAddress { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public Currency Currency { get; set; }
    }
}
