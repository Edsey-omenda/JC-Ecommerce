using JC_Ecommerce.Models.Domain;

namespace JC_Ecommerce.Repositories
{
    public interface IMpesaPaymentRepository
    {
        Task SavePaymentAsync(MpesaPayment payment);

        Task<MpesaPayment> GetPaymentAsync(string checkoutRequestId);
    }
}
