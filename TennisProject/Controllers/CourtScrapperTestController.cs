using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;
using Elastic.Clients.Elasticsearch.MachineLearning;
using System.Text;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces.Scrapper;
using Microsoft.EntityFrameworkCore;
using static TennisProject.Controllers.CourtScrapperTestController;
using Models.Models.Scrapper;
using Interfaces.Interfaces;
using Models.Models;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class CourtScrapperTestController : ControllerBase
    {
        private readonly TennisDbContext tennisDb;
        private readonly IScrapperManagerService scrapperManager;
        private readonly IBookingService bookingService;
        public CourtScrapperTestController(TennisDbContext tennisDb, IScrapperManagerService scrapperManager, IBookingService bookingService)
        {
            this.tennisDb = tennisDb;
            this.scrapperManager = scrapperManager;
            this.bookingService = bookingService;
        }

        [AllowAnonymous]
        [HttpPost("ScrapperBook")]
        public async Task<ScrapperBookResponse> ScrapperBook(BookGo2SportRequest request)
        {
            return await scrapperManager.BookGo2Sport(request);
        }

        [AllowAnonymous]
        [HttpPost("GetCourts")]
        public async Task<string> GetCourts(GetCourtsRequest requestBody)
        {
            string baseUrl = "http://prod.unicort.ru:8083/slots/";


            //var requestBody = new RequestBody
            //{
            //    TargetDate = "2024-09-25",
            //    ClubId = 177
            //};

            using var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUrl, content);

            var result = await response.Content.ReadAsStringAsync();


            var test = JsonConvert.DeserializeObject<List<Court>>(result);


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
                    !court.Slots.Any(slot => TimeOnly.FromTimeSpan(slot.TimeFrom) == _.StartTime && TimeOnly.FromTimeSpan(slot.TimeTo) == _.EndTime))
                .ToList(); //  A\C = Y (слоты которые недоступные для бронирования, (новые данные))


                var Z = occupiedByGo2Sport.Where(_ => !schedulesOccupied.Any(occ =>
                    occ.Schedule?.StartTime == _.StartTime &&
                    occ.Schedule?.EndTime == _.EndTime)).ToList(); // Y\B = Z (слоты которые нужно добавить в ScheduleOccupancies

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
            return result;
        }

        [AllowAnonymous]
        [HttpPost("PaymentStatus")]
        public async Task<GetScrapperPaymentStatusResponse> GetPaymentStatus(ScrapperPaymentRequest request)
        {
            return await scrapperManager.GetPaymentStatus(request);
        }



        public class GetCourtsRequest
        {
            [JsonProperty("target_date")]
            public DateTime TargetDate { get; set; }
            [JsonProperty("club_id")]
            public int ClubId { get; set; }
        }

        public class CourtResponse
        {
            [JsonProperty("courts")]
            public List<Court> Courts { get; set; }

            public CourtResponse()
            {
                Courts = new List<Court>();
            }

            // ...
        }

        public class Court
        {
            [JsonProperty("court_name")]
            public string CourtName { get; set; }

            [JsonProperty("go2sport_court_id")]
            public int Go2SportCourtId { get; set; }

            [JsonProperty("slots")]
            public List<Slot> Slots { get; set; }

            // ...
        }

        public class Slot
        {


            [JsonProperty("time_from")]
            public TimeSpan TimeFrom { get; set; }

            [JsonProperty("time_to")]
            public TimeSpan TimeTo { get; set; }

            [JsonProperty("price")]
            public decimal Price { get; set; }
        }


        public class Test
        {
            public string type { get; set; }

            [JsonProperty("event")]
            public Event Event { get; set; }
        }

        public class Event
        {
            public string club_id { get; set; }
            public string date { get; set; }

        }
    }
}
