namespace JC_Ecommerce.Models.Domain
{
    public class MpesaPayment
    {
        public Guid Id { get; set; }
        public string CheckoutId { get; set; }
        public string MerchantRequestId { get; set; }
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public decimal Amount { get; set; }
        public string MpesaReceiptNumber { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
