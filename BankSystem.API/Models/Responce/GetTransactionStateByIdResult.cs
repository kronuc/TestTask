using System.ComponentModel.DataAnnotations;

namespace SimpleBankSystem.API.Models
{
    public class GetTransactionStateByIdResult
    {
        public int RequestId { get; set; }
        public string State { get; set; }
    }
}
