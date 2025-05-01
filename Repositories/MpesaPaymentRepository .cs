using JC_Ecommerce.Data;
using JC_Ecommerce.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace JC_Ecommerce.Repositories
{
    public class MpesaPaymentRepository : IMpesaPaymentRepository
    {
        private readonly JCEcommerceDbContext jCEcommerceDbContext;

        public MpesaPaymentRepository(JCEcommerceDbContext jCEcommerceDbContext)
        {
            this.jCEcommerceDbContext = jCEcommerceDbContext;
        }
        public async Task<MpesaPayment> GetPaymentAsync(string checkoutRequestId)
        {
            return await jCEcommerceDbContext.MpesaPayments.FirstOrDefaultAsync(p => p.CheckoutId == checkoutRequestId);
        }

        public async Task SavePaymentAsync(MpesaPayment payment)
        {
            jCEcommerceDbContext.MpesaPayments.Add(payment);    

            await jCEcommerceDbContext.SaveChangesAsync();
        }
    }
}
