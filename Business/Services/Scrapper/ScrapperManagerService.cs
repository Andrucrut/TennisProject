using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces.Scrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Models.Scrapper;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Business.Services.Scrapper
{
    public class ScrapperManagerService : IScrapperManagerService
    {
       /// private static readonly HttpClient client = new HttpClient();
        private readonly IHttpClientFactory factory;
        private readonly TennisDbContext tennisDb;
        private readonly IConfiguration configuration;

        private string ScrapperBaseUrl;
        private string BookerBaseUrl;

        public ScrapperManagerService(TennisDbContext tennisDb, IHttpClientFactory factory, IConfiguration configuration)
        {
            this.tennisDb = tennisDb;
            this.factory = factory;
            this.configuration = configuration;
            ScrapperBaseUrl = configuration["ConnectionStrings:Scrapper"];
            BookerBaseUrl = configuration["ConnectionStrings:Booker"];
        }



        /// <summary>
        /// обнавляет в бд таблицу ScheduleOccupancies
        /// т.е добавляет туда данные о занятых кортах, и убирает если корт освободился
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSlots(GetScrapperCourtsRequest requestBody)
        {
            try
            {
                using var client = factory.CreateClient();
                string slotsUrl = ScrapperBaseUrl += "slots";
               // string baseUrl = "http://prod.unicort.ru:8083/slots/";


                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8,
                    "application/json");
                var response = await client.PostAsync(slotsUrl, content);

                var result = await response.Content.ReadAsStringAsync();


                var test = JsonConvert.DeserializeObject<List<ScrapperCourt>>(result);


                // to do задокументировать алгоритм
                foreach (var court in test) // court - C
                {
                    var schedules = await tennisDb.Schedules.Where(_ => _.Court.Go2SportId == court.Go2SportCourtId)
                        .ToListAsync(); // A

                    var schedulesOccupied = await tennisDb.ScheduleOccupancies.Include(_ => _.Schedule).Where(_ =>
                            _.Date.Date == requestBody.TargetDate.Date &&
                            _.Schedule.Court.Go2SportId == court.Go2SportCourtId)
                        .ToListAsync(); // B (недоступные слоты для бронирования лежащие в нашей бд (старые данные))

                    var occupiedByGo2Sport = schedules.Where(_ =>
                            !court.Slots.Any(slot =>
                                TimeOnly.FromTimeSpan(slot.TimeFrom) == _.StartTime &&
                                TimeOnly.FromTimeSpan(slot.TimeTo) == _.EndTime))
                        .ToList(); //  A\C = Y (слоты которые недоступные для бронирования, (новые данные))


                    var Z = occupiedByGo2Sport.Where(_ => !schedulesOccupied.Any(occ =>
                            occ.Schedule?.StartTime == _.StartTime &&
                            occ.Schedule?.EndTime == _.EndTime))
                        .ToList(); // Y\B = Z (слоты которые нужно добавить в ScheduleOccupancies

                    var W = schedulesOccupied.Where(b =>
                            !occupiedByGo2Sport.Any(y =>
                                y.StartTime == b.Schedule.StartTime && y.EndTime == b.Schedule.EndTime))
                        .ToList(); // B\Y = W (слоты которые нужно удалить из ScheduleOccupancies, потому что они не уже не заняты)


                    tennisDb.ScheduleOccupancies.RemoveRange(W);

                    //цикл для добавления Z в SchOcc

                    foreach (var z in Z)
                    {
                        await tennisDb.ScheduleOccupancies.AddAsync(new ScheduleOccupancy
                        {
                            ScheduleId = z.Id,
                            CreatedAt = DateTime.UtcNow,
                            Reason = OccupancyReasons.BookedByGo2Sport,
                            Date = requestBody.TargetDate
                        });
                    }

                    await tennisDb.SaveChangesAsync();

                    // потом мы обращаемся к бд и берем Sch, SchOcc, и обрабатываем для получпения доступных слотов?
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public async Task<ScrapperBookResponse> BookGo2Sport(BookGo2SportRequest request)
        {
            //if (request.Date.Date.ToUniversalTime() < DateTime.Today.Date.ToUniversalTime())
            //    return false;

            using var client = factory.CreateClient();
            var schedules = await tennisDb.Schedules.Where(_ => request.ScheduleIds.Contains(_.Id)).ToListAsync();

            //if (schedules == null)
            //    return false;

            var startTime = schedules.Min(_ => _.StartTime);
            var endTime = schedules.Max(_ => _.EndTime);

            var startTimeDate = request.Date.Add(startTime.ToTimeSpan());
            var endTimeDate = request.Date.Add(endTime.ToTimeSpan());

            var court = await tennisDb.Courts.Include(_ => _.CourtOrganization)
                .FirstOrDefaultAsync(_ => _.Id == schedules[0].CourtId);


            var scrapperRequestBody = new ScrapperBookRequest
            {
                CourtOrganizationGo2SportLink = court.CourtOrganization.Go2SportLink,
                StartDateTime = startTimeDate,
                EndDateTime = endTimeDate,
                CourtName = court.Name,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            string bookUrl = BookerBaseUrl += "slots/book";
          //  string baseUrl = "http://prod.unicort.ru:8083/slots/book";


            var content = new StringContent(JsonConvert.SerializeObject(scrapperRequestBody), Encoding.UTF8,
                "application/json");

       //     var json = JsonConvert.SerializeObject(scrapperRequestBody);
            var response = await client.PostAsync(bookUrl, content);

            var result = await response.Content.ReadAsStringAsync();


            var test = JsonConvert.DeserializeObject<ScrapperBookResponse>(result);

            return test;
        }

        public async Task<GetScrapperPaymentStatusResponse> GetPaymentStatus(ScrapperPaymentRequest request)
        {

            using var client = factory.CreateClient();
            string statusUrl = BookerBaseUrl += "status";
           // string baseUrl = "http://prod.unicort.ru:8083/payments/status";
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync(statusUrl, content);

            var result = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<string>(result);

            Enum.TryParse(result, true, out ScrapperPaymentStatus paymentStatus);
            return new GetScrapperPaymentStatusResponse
            {
                PaymentStatus = paymentStatus
            };
        }

        public async Task<bool> RefoundPayment(ScrapperPaymentRequest request)
        {
            try
            {
                using var client = factory.CreateClient();
                string refundUrl = BookerBaseUrl += "refund";
               // string baseUrl = "http://prod.unicort.ru:8083/payments/refund";
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(refundUrl, content);


                var resultString = await response.Content.ReadAsStringAsync();
                bool result;
                bool.TryParse(resultString, out result);

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
 
        }
    }



    public class ScrapperBookRequest
    {
        [JsonProperty("club_link")]
        public string CourtOrganizationGo2SportLink { get; set; }
        [JsonProperty("start")]
        public DateTime StartDateTime { get; set; }
        [JsonProperty("end")]
        public DateTime EndDateTime { get; set; }
        [JsonProperty("court")]
        public string CourtName { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }
    }

    //public class ScrapperBookResponse
    //{
    //    [JsonProperty("payment_link")]
    //    public string PaymentLink { get; set; }
    //    [JsonProperty("order_id")]
    //    public long OrderId { get; set; }
    //}




}
