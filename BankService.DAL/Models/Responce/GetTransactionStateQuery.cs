using System.ComponentModel.DataAnnotations;

namespace BankSystem.API.Models.Request
{
    public class GetTransactionStateResult
    {
        public int ClientId { get; set; }

        public string DepartmentAddress { get; set; }
        public string State { get; set; }
    }
}
