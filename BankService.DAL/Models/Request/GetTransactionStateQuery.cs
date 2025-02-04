using System.ComponentModel.DataAnnotations;

namespace BankService.DAL.Models
{
    public class GetTransactionStateQuery
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public string DepartmentAddress { get; set; }
    }
}
