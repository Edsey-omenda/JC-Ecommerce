namespace JC_Ecommerce.Models.DTOs
{
    public class StkPushRequestDto
    {
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string AccountReference { get; set; }
        public string TransactionDesc { get; set; }
    }
}
