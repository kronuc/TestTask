using System.ComponentModel.DataAnnotations;

namespace SimpleBankSystem.API.Models
{
    public class GetTransactionStateByIdResult
    {
        [Required]
        public int RequestId { get; set; }
        public string State { get; set; }
    }
}
