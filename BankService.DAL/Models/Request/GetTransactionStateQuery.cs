using System.ComponentModel.DataAnnotations;

namespace BankSystem.API.Models.Request
{
    public class GetTransactionStateQuery
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public string DepartmentAddress { get; set; }
    }
}
