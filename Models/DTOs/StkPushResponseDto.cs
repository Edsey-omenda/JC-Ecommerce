namespace JC_Ecommerce.Models.DTOs
{
    public class StkPushResponseDto
    {
        public string MerchantRequestId { get; set; }
        public string CheckoutRequestId { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string CustomerMessage { get; set; }
    }
}
