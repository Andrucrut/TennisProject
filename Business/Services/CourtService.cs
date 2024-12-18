using Business.Mappings.Maps;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dtos;
using Models.Models;
using Newtonsoft.Json;
using System.Text;
using Interfaces.Interfaces.Scrapper;
using Models.Models.Paging;
using Models.Models.Scrapper;
using Business.Helpers;

namespace Business.Services
{
    public class CourtService : ICourtService
    {
        private readonly TennisDbContext tennisDb;
        private readonly ILogService logService;
        private readonly IScrapperManagerService scrapperManager;
        private readonly CourtOrganizationMap courtOrganizationMap;
        protected ILogger Logger { get; set; }
        public CourtService(TennisDbContext tennisDb, ILogger<UserService> logger, ILogService logService,
            CourtOrganizationMap courtOrganizationMap, IScrapperManagerService scrapperManager)
        {
            this.tennisDb = tennisDb;
            Logger = logger;
            this.logService = logService;
            this.courtOrganizationMap = courtOrganizationMap;
            this.scrapperManager = scrapperManager;
        }

        public async Task<GetCourtDictionaryByCityResponse> GetCourtDictionaryByCityId(GetCourtDictionaryByCityRequest request)
        {
            
            var courtDicts = await tennisDb.CourtDictionaries.Where(_ => _.CityId == request.CityId).Select(cd => new CourtDictInfo
            {
                Id = cd.Id,
                Name = cd.Name
            }).ToListAsync();

            return new GetCourtDictionaryByCityResponse { CourtDicts = courtDicts, Success = true };
        }
        

        public async Task<ResponseBase> AddOrganization(AddCourtOrganizationRequest request)
        {
            try
            {
                await tennisDb.CourtOrganizations.AddAsync(request.Organization);
                await tennisDb.SaveChangesAsync();
                return new ResponseBase { Success = true, Message = "Added" };
            }
            catch (Exception ex)
            {
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            }
        }

        public async Task<string> UpdateOrganization(UpdateCourtOrganizationRequest request)
        {
            var organization = await tennisDb.CourtOrganizations.FirstOrDefaultAsync(_ => _.Id == request.Organization.Id);
            if (organization == null)
                return "Organization doesn't exist";
            organization.PhoneNumber = request.Organization.PhoneNumber;
            organization.Address = request.Organization.Address;
            organization.Description = request.Organization.Description;
            organization.WebSiteLink = request.Organization.WebSiteLink;
            organization.CityId = request.Organization.CityId;
            organization.DistrictId = request.Organization.DistrictId;
            organization.Name = request.Organization.Name;
            organization.PhoneNumber = request.Organization.PhoneNumber;
            await tennisDb.SaveChangesAsync();
            return "Updated";
        }

        public async Task<string> RemoveOrganization(int id)
        {
            var organization = await tennisDb.CourtOrganizations.FirstOrDefaultAsync(_ => _.Id == id);
            if (organization == null)
                return "Organization doesn't exist";
            tennisDb.CourtOrganizations.Remove(organization);
            await tennisDb.SaveChangesAsync();
            return "Removed";
        }
        public async Task<string> AddCourt(Court request)
        {
            var organization = await tennisDb.CourtOrganizations.FirstOrDefaultAsync(_ => _.Id == request.CourtOrganizationId);
            if (organization != null)
            {
                try
                {
                    await tennisDb.Courts.AddAsync(request);
                    await tennisDb.SaveChangesAsync();
                    return "Added";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "Organization doesn't exist";
        }

        public async Task<string> EditCourt(Court request)
        {
            var court = await tennisDb.Courts.FirstOrDefaultAsync(_ => _.Id == request.Id);
            if (court == null)
                return "Court doesn't exist";
            court.Number = request.Number;
            court.CourtOrganizationId = request.CourtOrganizationId;
            court.SurfaceType = request.SurfaceType;
            await tennisDb.SaveChangesAsync();
            return "Updated";
        }

        public async Task<string> RemoveCourt(int id)
        {
            var court = await tennisDb.Courts.FirstOrDefaultAsync(_ => _.Id == id);
            if (court == null)
                return "Court doesn't exist";
            tennisDb.Courts.Remove(court);
            await tennisDb.SaveChangesAsync();
            return "Removed";
        }

        public async Task<GerCourtOrganizationsResponse> GetCourtOrganizations(GetCourtOranizationRequest request)
        {
            // TO DO В БУДУЩЕМ НУЖНА ОПТИМИЗАЦИЯ ЗАПРОСОВ!!!!!!!!!!!!!!!!!!!!!!!
            
            // ОПТИМИЗАЦИЯ ЗАПРОСОВ!!!!!!!!!!!!!!!
            try
            {
                var query = tennisDb.CourtOrganizations
                    .Include(_ => _.City).Include(_ => _.District).Where(_ => _.CityId == request.CityId)
                    .AsQueryable();

                var total = await query.CountAsync();
            
                var pagedResult = query
                    .Skip(request.Skip.Value)
                    .Take(request.Take.Value);

                
                var organizations =  await pagedResult.ToListAsync();

                var dtos = organizations.Select(_ => courtOrganizationMap.MapCore(_)).ToList();
                foreach (var dto in dtos)
                {
                    var courtIds = await tennisDb.Courts.Where(_ => _.CourtOrganizationId == dto.Id).Select(_ => _.Id).ToListAsync();
                    dto.CourtsAmount = courtIds.Count();

                    var minStartTime = await tennisDb.Schedules
                        .Where(s => courtIds.Contains((int)s.CourtId)).MinAsync(s => s.StartTime);

                    var maxEndTime = await tennisDb.Schedules
                        .Where(s => courtIds.Contains((int)s.CourtId))
                        .MaxAsync(s => s.EndTime);

                    // Обновляем dto с полученными значениями
                    dto.StartTime = minStartTime;
                    dto.EndTime = maxEndTime;
                }


                var pagedList = new PagedList<CourtOrganizationDto>(dtos, total, request);
                return new GerCourtOrganizationsResponse { CourtOrganizations = pagedList, Success = true};
            }
            catch (Exception ex)
            {
                return new GerCourtOrganizationsResponse { Success = false, ExceptionMess = ex.Message };
            }

        }

        public async Task<GetCourtOrganizationByIdResponse> GetCourtOrganizationById(int id)
        {
            var organization = await tennisDb.CourtOrganizations.Include(_ => _.District).FirstOrDefaultAsync(_ => _.Id == id);
            if (organization == null)
                return new GetCourtOrganizationByIdResponse { Success = false, ExceptionMess = $"Organization has not found by id: {id}" };

            var dto = courtOrganizationMap.MapCore(organization);
            var courtIds = await tennisDb.Courts.Where(_ => _.CourtOrganizationId == organization.Id).Select(_ => _.Id).ToListAsync();
            dto.CourtsAmount = courtIds.Count();

            var minStartTime = await tennisDb.Schedules
                .Where(s => courtIds.Contains((int)s.CourtId))
                .MinAsync(s => s.StartTime);

            var maxEndTime = await tennisDb.Schedules
                .Where(s => courtIds.Contains((int)s.CourtId))
                .MaxAsync(s => s.EndTime);

            // Обновляем dto с полученными значениями
            dto.StartTime = minStartTime;
            dto.EndTime = maxEndTime;

            return new GetCourtOrganizationByIdResponse
            {
                Success = true,
                Organization = dto,
                CourtIds = courtIds
            };
        }

        public async Task<GetCourtByIdResponse> GetCourtById(int id)
        {
            var court = await tennisDb.Courts.FirstOrDefaultAsync(_ => _.Id == id);
            if (court == null)
                return new GetCourtByIdResponse { Success = false, ExceptionMess = $"Organization has not found by id: {id}" };
            return new GetCourtByIdResponse { Success = true, Court = court };
        }

        public async Task<ResponseBase> AddSchedule(List<AddScheduleRequest> request)
        {
            // To do: validate if this timeslots already exist!!! but for pre-mvp fuck it
            var schedules = request.Select(request => new Schedule
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PriceJson = request.Price,
            //    Price = request.Price,
                CourtId = request.CourtId
            }).ToList();
            //var schedules = new Schedule
            //{
            //    StartTime = request.StartTime,
            //    EndTime = request.EndTime,
            //    Price = request.Price,
            //    CourtId = request.CourtId
            //};

            try
            {
                await tennisDb.Schedules.AddRangeAsync(schedules);
                await tennisDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ResponseBase { Success = false, ExceptionMess = ex.Message };
            }
            return new ResponseBase { Success = true, Message = "Added" };
        }

        public async Task<GetAllScheduleResponse> GetAllSchedule()
        {
            var schedules = await tennisDb.Schedules.ToListAsync();
            return new GetAllScheduleResponse { Schedules = schedules, Success = true };
        }
        //public async Task<GetAvailableTimeSlotsResponse> GetAvailableTimeSlots(GetAvailableTimeSlotsRequest request)
        //{
        //    try
        //    {
        //        var utcDates = request.Dates.Select(d => d.Date.Date.ToUniversalTime()).ToList();

        //        //var bookedScheduleIds = await tennisDb.Bookings
        //        //    .Where(b => utcDates.Contains(b.Date.Date.ToUniversalTime())&& b.Status == BookingStatus.Booked)
        //        //    .SelectMany(b => b.Schedules.Select(s => s.Id))
        //        //    .ToListAsync();

        //        var occupiedScheduleIds = await tennisDb.ScheduleOccupancies
        //            .Where(_ => utcDates.Contains(_.Date.Date.ToUniversalTime())).Select(_ => _.ScheduleId)
        //            .ToListAsync();



        //        var courtIds = request?.CourtsIds;
        //        if (request.OrganizationIds != null)
        //        {
        //            courtIds = await tennisDb.Courts.Where(_ => request.OrganizationIds.Contains((int)_.CourtOrganizationId)).Select(_ => _.Id).ToListAsync();
        //        }

        //        var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        //        var currentTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);

        //        var currentMoscowTimeOnly = TimeOnly.FromDateTime(currentTimeMoscow);

        //        var availableTimeSlots = await tennisDb.Schedules.Include(_ => _.Court)
        //            .Where(s => courtIds.Contains(s.CourtId.Value))
        //            .Where(s => !occupiedScheduleIds.Contains(s.Id))
        //            .Where(s => s.StartTime > currentMoscowTimeOnly)
        //            .OrderBy(s => s.StartTime)

        //            // Исключаем расписания с забронированными слотами
        //            //.Select(s => new Schedule { StartTime = s.StartTime, EndTime = s.EndTime,  })
        //            .ToListAsync();

        //        var result = new List<ScheduleDto>();

        //        foreach (var timeSlot in availableTimeSlots)
        //        {
        //            var scheduleDto = new ScheduleDto
        //            {
        //                //Ids = new List<long>().Add(timeSlot.Id)
        //                Id = timeSlot.Id,
        //                StartTime = timeSlot.StartTime,
        //                EndTime = timeSlot.EndTime,
        //                Price = timeSlot.PriceJson.Friday, // change to asked date day
        //                CourtId = (int)timeSlot.CourtId,
        //                Court = new CourtDto
        //                {
        //                    CourtId = (int)timeSlot.CourtId,
        //                    CourtName = timeSlot.Court.Name,
        //                    CourtType = timeSlot.Court.CourtType,
        //                    SurfaceType = timeSlot.Court.SurfaceType,
        //                }
        //            };
        //            result.Add(scheduleDto);
        //        }

        //        await logService.AddLog(new Log
        //        {
        //            Time = DateTime.UtcNow,
        //            Request = JsonConvert.SerializeObject(request),
        //            Controller = "CourtController/GetAvailableTimeSlots",
        //            Service = "GetAvailableTimeSlots",
        //            LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
        //            Response = JsonConvert.SerializeObject(availableTimeSlots),
        //        });
        //        return new GetAvailableTimeSlotsResponse { Success = true, TimeSlots = result };
        //    }
        //    catch (Exception ex)
        //    {
        //        await logService.AddLog(new Log
        //        {
        //            Time = DateTime.UtcNow,
        //            Request = JsonConvert.SerializeObject(request),
        //            Controller = "CourtController/GetAvailableTimeSlots",
        //            Service = "GetAvailableTimeSlots",
        //            LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
        //            Message = ex.Message,
        //        });
        //        return new GetAvailableTimeSlotsResponse { Success = false, ExceptionMess = ex.Message };
        //    }
     

        //}



        public async Task<GetAvailableTimeSlotsResponse> GetAvailableTimeSlots(GetAvailableTimeSlotsRequest request)
        {
            try
            {
                //var utcDates = request.Dates.Select(d => d.Date.Date.ToUniversalTime()).ToList();
                var utcDate = request.Date.Date.ToUniversalTime();



                var courtIds = request?.CourtsIds;
                if (request.OrganizationIds != null)
                {
                    courtIds = await tennisDb.Courts
                        .Where(_ => request.OrganizationIds.Contains((int)_.CourtOrganizationId))
                        .Select(_ => _.Id)
                        .ToListAsync();
                }


                var go2SportClubId = await tennisDb.Courts.Include(_ => _.CourtOrganization)
                    .Where(_ => _.Id == courtIds[0]).Select(_ => _.CourtOrganization.Go2SportId).FirstOrDefaultAsync();

                if (go2SportClubId != null)
                {
                    var pingScrapperRequest = new GetScrapperCourtsRequest
                    {
                        ClubId = (int)go2SportClubId,
                        TargetDate = request.Date
                    };
                    var pingScrapperResponse = await scrapperManager.UpdateSlots(pingScrapperRequest);
                    if (pingScrapperResponse == false)
                        return new GetAvailableTimeSlotsResponse
                            { Success = false, Message = "Couldn't scrapp go2sport" };
                }




                var occupiedScheduleIds = await tennisDb.ScheduleOccupancies
                    .Where(_ => utcDate == _.Date.Date.ToUniversalTime())
                    .Select(_ => _.ScheduleId)
                    .ToListAsync();

                var moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                var currentTimeMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);
                var currentMoscowTimeOnly = TimeOnly.FromDateTime(currentTimeMoscow);

                if(utcDate.Date > DateTime.UtcNow.ToUniversalTime().Date)
                    currentMoscowTimeOnly = new TimeOnly(0, 0);


                var availableTimeSlots = await tennisDb.Schedules
                    .Include(_ => _.Court)
                    .Where(s => courtIds.Contains(s.CourtId.Value))
                    .Where(s => !occupiedScheduleIds.Contains(s.Id))
                    .Where(s => s.StartTime > currentMoscowTimeOnly)
                    .OrderBy(s => s.StartTime)
                    .ToListAsync();


                var scheduleDtos = new List<ScheduleDto>();

                foreach (var courtId in courtIds)
                {
                    availableTimeSlots.Where(_ => _.CourtId == courtId).ToList();

                    /* for (var i = 0; i < availableTimeSlots.Where(_ => _.CourtId == courtId).ToList().Count - 1; i++)
                     {
                         var schedule = availableTimeSlots[i];
                         var scheduleNext = availableTimeSlots[i + 1];

                         if (schedule.StartTime.Minute == 0 &&
                             scheduleNext.EndTime - schedule.StartTime == TimeSpan.FromHours(1))
                         {
                             var ids = new List<long>();
                             ids.Add(schedule.Id);
                             ids.Add(scheduleNext.Id);
                             scheduleDtos.Add(new ScheduleDto
                             {
                                 Ids = ids,
                                 StartTime = schedule.StartTime,
                                 EndTime = scheduleNext.EndTime,
                                 CourtId = courtId,
                                // CourtId = schedule.CourtId,
                                Price = schedule.PriceJson.Friday
                             });
                         }
                         if(schedule.StartTime.Minute == 0 &&
                            schedule.EndTime - schedule.StartTime == TimeSpan.FromHours(1))
                         {
                             var ids = new List<long>();
                             ids.Add(schedule.Id);
                             scheduleDtos.Add(new ScheduleDto
                             {
                                 Ids = ids,
                                 StartTime = schedule.StartTime,
                                 EndTime = schedule.EndTime,
                                 CourtId = courtId,
                                 // CourtId = schedule.CourtId,
                                 Price = schedule.PriceJson.Friday
                             });
                         }

                         if (i == availableTimeSlots.Where(_ => _.CourtId == courtId).ToList().Count - 2)
                         {
                             if (scheduleNext.StartTime.Minute == 0 &&
                                 scheduleNext.EndTime - scheduleNext.StartTime == TimeSpan.FromHours(1))
                             {
                                 var ids = new List<long>();
                                 ids.Add(scheduleNext.Id);
                                 scheduleDtos.Add(new ScheduleDto
                                 {
                                     Ids = ids,
                                     StartTime = scheduleNext.StartTime,
                                     EndTime = scheduleNext.EndTime,
                                     CourtId = courtId,
                                     // CourtId = schedule.CourtId,
                                     Price = scheduleNext.PriceJson.Friday
                                 });
                             }
                         }
                     } */

                    var dtos = await CreateScheduleDtos(availableTimeSlots.Where(_ => _.CourtId == courtId).ToList(), request.Date);
                    scheduleDtos.AddRange(dtos);

                }


                //var groupedSchedules = GroupSchedulesByHour(availableTimeSlots);

                //var result = ConvertToScheduleDtos(groupedSchedules);

                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "CourtController/GetAvailableTimeSlots",
                    Service = "GetAvailableTimeSlots",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Info,
                    Response = JsonConvert.SerializeObject(scheduleDtos),
                });

                return new GetAvailableTimeSlotsResponse { Success = true, TimeSlots = scheduleDtos };
            }
            catch (Exception ex)
            {
                await logService.AddLog(new Log
                {
                    Time = DateTime.UtcNow,
                    Request = JsonConvert.SerializeObject(request),
                    Controller = "CourtController/GetAvailableTimeSlots",
                    Service = "GetAvailableTimeSlots",
                    LogLevel = Infrastructure.Data.Entities.LogLevel.Error,
                    Message = ex.Message,
                });
                return new GetAvailableTimeSlotsResponse { Success = false, ExceptionMess = ex.Message };
            }
        }

        private async Task<List<ScheduleDto>> CreateScheduleDtos(List<Schedule> schedules, DateTime date)
        {
            var scheduleDtos = new List<ScheduleDto>();

            for (int i = 0; i < schedules.Count - 1; i++)
            {
                var schedule = schedules[i];

                if (schedule.StartTime.Minute == 0 &&
                    schedule.EndTime - schedule.StartTime == TimeSpan.FromHours(1))
                {
                    scheduleDtos.Add(CreateSingleHourSlot(schedule, date));
                }

                var nextSchedule = schedules[i + 1];

                if (schedule.StartTime.Minute == 0 &&
                    nextSchedule.EndTime - schedule.StartTime == TimeSpan.FromHours(1))
                {
                    scheduleDtos.Add(CreateTwoHourSlot(schedule, nextSchedule, date));
                }

                if (i == schedules.Count - 2)
                {
                    if (nextSchedule.StartTime.Minute == 0 &&
                        nextSchedule.EndTime - nextSchedule.StartTime == TimeSpan.FromHours(1))
                    {
                        scheduleDtos.Add(CreateSingleHourSlot(nextSchedule, date));
                    }
                }
            }
            return scheduleDtos;
        }

        private ScheduleDto CreateSingleHourSlot(Schedule schedule, DateTime date)
        {
            return new ScheduleDto
            {
                Ids = new List<long> { schedule.Id },
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                CourtId = schedule.CourtId.Value,
                Price = PriceForDay.GetPriceForDay(schedule.PriceJson, date.DayOfWeek),
                CourtName = schedule.Court?.Name,
                CourtNumber = schedule.Court?.Number,
                SurfaceType = schedule.Court?.SurfaceType,
                CourtType = schedule.Court?.CourtType,

            };
        }

        private ScheduleDto CreateTwoHourSlot(Schedule schedule, Schedule nextSchedule, DateTime date)
        {
            return new ScheduleDto
            {
                Ids = new List<long> { schedule.Id, nextSchedule.Id },
                StartTime = schedule.StartTime,
                EndTime = nextSchedule.EndTime,
                CourtId = schedule.CourtId.Value,
                Price = PriceForDay.GetPriceForDay(schedule.PriceJson, date.DayOfWeek) + PriceForDay.GetPriceForDay(nextSchedule.PriceJson, date.DayOfWeek),
                CourtName = schedule.Court?.Name,
                CourtNumber = schedule.Court?.Number,
                SurfaceType = schedule.Court?.SurfaceType,
                CourtType = schedule.Court?.CourtType,
            };
        }



    }


}