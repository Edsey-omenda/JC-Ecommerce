using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;
using JC_Ecommerce.Repositories;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using MailKit.Net.Smtp;

namespace JC_Ecommerce.Services
{
    public class MpesaService : IMpesaService
    {
        private readonly IMpesaPaymentRepository mpesaPaymentRepository;
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public MpesaService(IMpesaPaymentRepository mpesaPaymentRepository, IConfiguration configuration, HttpClient httpClient)
        {
            this.mpesaPaymentRepository = mpesaPaymentRepository;
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var consumerKey = configuration["Mpesa:ConsumerKey"];
            var consumerSecret = configuration["Mpesa:ConsumerSecret"];
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}"));

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await httpClient.GetAsync($"{configuration["Mpesa:BaseUrl"]}/oauth/v1/generate?grant_type=client_credentials");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to obtain Mpesa access token: {responseBody}");
            }

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var token = jsonDoc.RootElement.GetProperty("access_token").GetString();

            return token;
        }
        public async Task<MpesaPayment> GetPaymentByCheckoutIdAsync(string checkoutRequestId)
        {
            return await mpesaPaymentRepository.GetPaymentAsync(checkoutRequestId);
        }

        public async Task<StkPushResponseDto> InitiateStkPushAsync(StkPushRequestDto requestDto)
        {
            // TODO: Safaricom STK push integration
            var accessToken = await GetAccessTokenAsync();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var shortcode = configuration["Mpesa:ShortCode"];
            var passkey = configuration["Mpesa:PassKey"];
            var password = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{shortcode}{passkey}{timestamp}"));


            var stkPushRequest = new
            {
                BusinessShortCode = shortcode,
                Password = password,
                Timestamp = timestamp,
                TransactionType = "CustomerPayBillOnline",
                Amount = requestDto.Amount,
                PartyA = requestDto.PhoneNumber,
                PartyB = shortcode,
                PhoneNumber = requestDto.PhoneNumber,
                CallBackURL = configuration["Mpesa:CallbackUrl"],
                AccountReference = "JC_Ecommerce",
                //TransactionDesc = requestDto.Description ?? "Payment"
            };

            var jsonRequest = JsonSerializer.Serialize(stkPushRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{configuration["Mpesa:BaseUrl"]}/mpesa/stkpush/v1/processrequest", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Mpesa STK push failed: {responseBody}");
            }

            var stkResponse = JsonSerializer.Deserialize<StkPushResponseDto>(responseBody);

            // Save to database
            var payment = new MpesaPayment
            {
                Id = Guid.NewGuid(),
                CheckoutId = stkResponse.CheckoutRequestId,
                MerchantRequestId = stkResponse.MerchantRequestId,
                ResultCode = stkResponse.ResponseCode,
                ResultDesc = stkResponse.ResponseDescription,
                Amount = requestDto.Amount,
                MpesaReceiptNumber = "", // will be updated later
                PhoneNumber = requestDto.PhoneNumber,
                DateCreated = DateTime.UtcNow
            };

            await mpesaPaymentRepository.SavePaymentAsync(payment);

            return stkResponse;
        }
    }
    }
}
