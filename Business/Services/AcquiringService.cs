using Interfaces.Interfaces;
using Microsoft.Extensions.Configuration;
using Models.Models.Payment;
using Models.Models.Scrapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class AcquiringService : IAcquiringService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory factory;
        private readonly ILogService logService;
        private string AcquiringBaseUrl; 
        public AcquiringService(IHttpClientFactory factory, IConfiguration configuration, ILogService logService) 
        {
            this.factory = factory;
            this.configuration = configuration;
            this.logService = logService;
            AcquiringBaseUrl = configuration["ConnectionStrings:Acquiring"];
        }
        public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request)
        {
            try
            {
                using var client = factory.CreateClient();
                string paymentUrl = AcquiringBaseUrl + "Payment/CreatePayment";

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                   "application/json");
                var response = await client.PostAsync(paymentUrl, content);

                var result = await response.Content.ReadAsStringAsync();


                return JsonConvert.DeserializeObject<CreatePaymentResponse>(result);
            }
            catch (Exception ex)
            {
                return new CreatePaymentResponse { ExceptionMess = ex.Message, Success = false };
            }
        }

        public async Task<GetPaymentStatusResponse> GetPaymentStatus(GetPaymentStatusRequest request)
        {
            try
            {
                using var client = factory.CreateClient();
                string statusUrl = AcquiringBaseUrl + "Payment/GetStatus";

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                   "application/json");
                var response = await client.PostAsync(statusUrl, content);

                var result = await response.Content.ReadAsStringAsync();


                return JsonConvert.DeserializeObject<GetPaymentStatusResponse>(result);
            }
            catch (Exception ex)
            {
                return new GetPaymentStatusResponse { ExceptionMess = ex.Message, Success = false };
            }
        }


        public async Task<GetPaymentStatusResponse> CapturePayment(GetPaymentStatusRequest request)
        {
            try
            {
                using var client = factory.CreateClient();
                string statusUrl = AcquiringBaseUrl + "Payment/Capture";

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                   "application/json");
                var response = await client.PostAsync(statusUrl, content);

                var result = await response.Content.ReadAsStringAsync();


                return JsonConvert.DeserializeObject<GetPaymentStatusResponse>(result);
            }
            catch (Exception ex)
            {
                return new GetPaymentStatusResponse { ExceptionMess = ex.Message, Success = false };
            }
        }

        public async Task<CancelPaymentResponse> CancelPayment(RefundPaymentRequest request)
        {
            try
            {
                var statusResponse = await GetPaymentStatus(new GetPaymentStatusRequest { PaymentId = request.PaymentId });
                if (statusResponse.Status.ToLower() == "succeeded")
                {
                    var refundResponse = await Refund(request);
                    await logService.AddLog(new Infrastructure.Data.Entities.Log 
                    { 
                        Request = JsonConvert.SerializeObject(request),
                        Response = JsonConvert.SerializeObject(refundResponse),
                        LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                        Service = "AcquiringService/CancelPayment",
                        Time = DateTime.UtcNow
                    });
                    return new CancelPaymentResponse { Success = refundResponse.Success, ExceptionMess = refundResponse.ExceptionMess };
                }
                else if (statusResponse.Status.ToLower() == "waitingforcapture")
                {
                    var cancelResponse = await Cancel(new GetPaymentStatusRequest { PaymentId = request.PaymentId });
                    await logService.AddLog(new Infrastructure.Data.Entities.Log
                    {
                        Request = JsonConvert.SerializeObject(request),
                        Response = JsonConvert.SerializeObject(cancelResponse),
                        LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                        Service = "AcquiringService/CancelPayment",
                        Time = DateTime.UtcNow
                    });
                    return new CancelPaymentResponse { Success = cancelResponse.Success, ExceptionMess = cancelResponse.ExceptionMess };
                }
                return new CancelPaymentResponse { Success = false, ExceptionMess = "ex.Message" };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Infrastructure.Data.Entities.Log
                {
                    Request = JsonConvert.SerializeObject(request),
                    Response = JsonConvert.SerializeObject(ex.Message),
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    Service = "AcquiringService/CancelPayment",
                    Time = DateTime.UtcNow,
                    Message = ex.Message
                });
                return new CancelPaymentResponse { Success = false, ExceptionMess = ex.Message };
            }
        }

        public async Task<GetPaymentStatusResponse> Refund(RefundPaymentRequest request)
        {
            try
            {
                using var client = factory.CreateClient();
                string statusUrl = AcquiringBaseUrl + "Payment/Refund";

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                   "application/json");
                var response = await client.PostAsync(statusUrl, content);

                var result = await response.Content.ReadAsStringAsync();


                return JsonConvert.DeserializeObject<GetPaymentStatusResponse>(result);
            }
            catch (Exception ex)
            {
                return new GetPaymentStatusResponse { ExceptionMess = ex.Message, Success = false };
            }
        }

        public async Task<GetPaymentStatusResponse> Cancel(GetPaymentStatusRequest request)
        {
            try
            {
                using var client = factory.CreateClient();
                string statusUrl = AcquiringBaseUrl + "Payment/Cancel";

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                   "application/json");
                var response = await client.PostAsync(statusUrl, content);

                var result = await response.Content.ReadAsStringAsync();


                return JsonConvert.DeserializeObject<GetPaymentStatusResponse>(result);
            }
            catch (Exception ex)
            {
                return new GetPaymentStatusResponse { ExceptionMess = ex.Message, Success = false };
            }
        }

    }
}
