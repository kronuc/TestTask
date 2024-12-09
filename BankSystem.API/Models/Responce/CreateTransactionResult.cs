using System.ComponentModel.DataAnnotations;

namespace SimpleBankSystem.API.Models
{
    public class CreateTransactionResult
    {
        public int ClientId { get; set; }
        
        public string DepartmentAddress { get; set; }

        public double Amount { get; set; }

        public Currency Currency {  get; set; }
    }
}
