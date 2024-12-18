using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using Models.Models;
using Models.Models.Paging;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class CourtController : ControllerBase
    {
        public ICourtService courtService { get; set; }
        public TennisDbContext db { get; set; }

        public CourtController(ICourtService courtService, TennisDbContext db)
        {
            this.courtService = courtService;
            this.db = db;
        }

        [AllowAnonymous]
        [HttpPost("AddOrganization")]
        public async Task<ResponseBase> AddCourtOrganization([FromBody] AddCourtOrganizationRequest request,
            [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, Message = "Not validated" };

            return await courtService.AddOrganization(request);
        }

        [AllowAnonymous]
        [HttpPost("AddAdditionalServices")]
        public async Task<ResponseBase> AddAdditionalServices([FromBody] AddAdditionalServicesRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, Message = "Not validated" };

            var courtOrganization = await db.CourtOrganizations.FirstOrDefaultAsync(_ => _.Id == request.CourOrganizationId);

            courtOrganization.AdditionalServices = request.AdditionalServices;

            await db.SaveChangesAsync();

            return new ResponseBase { Success = true };
        }


        [AllowAnonymous]
        [HttpPost("AddPhotoUrls")]
        public async Task<ResponseBase> AddPhotoUrls([FromBody] AddPhotoUrlsRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, Message = "Not validated" };

            var courtOrganization = await db.CourtOrganizations.FirstOrDefaultAsync(_ => _.Id == request.CourOrganizationId);

            if (courtOrganization.PhotoUrls != null)
                courtOrganization.PhotoUrls.AddRange(request.PhotoUrls);
            else
                courtOrganization.PhotoUrls = request.PhotoUrls;
            await db.SaveChangesAsync();

            return new ResponseBase { Success = true };
        }

        [AllowAnonymous]
        [HttpPost("GetOrganizations")]
        public async Task<GerCourtOrganizationsResponse> GetCourtOrganizations(
            [FromServices]  InitDataValidator initDataValidator, [FromBody] GetCourtOranizationRequest request)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GerCourtOrganizationsResponse { Success = false, Message = "Not validated" };
            request.Calculate();
            return await courtService.GetCourtOrganizations(request);
        }

        [AllowAnonymous]
        [HttpGet("GetOrganizationById")]
        public async Task<GetCourtOrganizationByIdResponse> GetCourtOrganizationById([FromBody] int id,
            [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetCourtOrganizationByIdResponse { Success = false, Message = "Not validated" };

            return await courtService.GetCourtOrganizationById(id);
        }


    [AllowAnonymous]
        [HttpPost("PostCourtDictionaryByCityId")]
        public async Task<GetCourtDictionaryByCityResponse> GetCourtDictionaryByCityId([FromBody] GetCourtDictionaryByCityRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
               return new GetCourtDictionaryByCityResponse { Success = false, Message = "Not validated" };

            return await courtService.GetCourtDictionaryByCityId(request);
        }

        [AllowAnonymous]
        [HttpGet("GetCourtById")]
        public async Task<GetCourtByIdResponse> GetCourtById([FromBody] int id, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetCourtByIdResponse { Success = false, Message = "Not validated" };

            return await courtService.GetCourtById(id);
        }

        [AllowAnonymous]
        [HttpPost("AddCourt")]
        public async Task<string> AddCourt([FromBody] Court request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return "Not validated";
                //return new ResponseBase { Success = false, Message = "Not validated" };

            return await courtService.AddCourt(request);
        }

        [AllowAnonymous]
        [HttpPost("AddSchedule")]
        public async Task<ResponseBase> AddSchedule([FromBody] List<AddScheduleRequest> request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ResponseBase { Success = false, Message = "Not validated" };

            return await courtService.AddSchedule(request);
        }

        [AllowAnonymous]
        [HttpGet("GetAllSchedule")]
        public async Task<GetAllScheduleResponse> GetAllSchedule([FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetAllScheduleResponse { Success = false, Message = "Not validated" };

            return await courtService.GetAllSchedule();
        }

        [AllowAnonymous]
        [HttpPost("GetAvailableTimeSlots")]
        public async Task<GetAvailableTimeSlotsResponse> GetAvailableTimeSlots([FromBody] GetAvailableTimeSlotsRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new GetAvailableTimeSlotsResponse { Success = false, Message = "Not validated" };

            return await courtService.GetAvailableTimeSlots(request);
        }



        [AllowAnonymous]
        [HttpPost("AddGeoData")]
        public async Task<bool> AddGeoData([FromBody] AddGeoDataRequest request)
        {
            var court = await db.CourtOrganizations.FirstOrDefaultAsync(_ => _.Id == request.CourOrganizationId);
            court.GeoData = request.GeoData;
            await db.SaveChangesAsync();
            return true; 
        }


    }

    public class AddGeoDataRequest
    {
        public long CourOrganizationId { get; set; }
        public GeoData GeoData { get; set; }
    }

    public class AddAdditionalServicesRequest
    {
        public long CourOrganizationId { get; set; }
        public AdditionalServices AdditionalServices { get; set; }
    }

    public class AddPhotoUrlsRequest
    {
        public long CourOrganizationId { get; set; }
        public List<string> PhotoUrls { get; set; }
    }
}


// to do:
// get available time slots for specific court requested day
// get available time slots for all courts for specific day?
