using Infrastructure.Data.Entities;
using Models.Dtos;
using Models.Models;
using Models.Models.Paging; 

namespace Interfaces.Interfaces
{
    public interface ICourtService
    {
        public Task<GetCourtDictionaryByCityResponse> GetCourtDictionaryByCityId(GetCourtDictionaryByCityRequest request);
        public Task<ResponseBase> AddOrganization(AddCourtOrganizationRequest organization);
        public Task<string> UpdateOrganization(UpdateCourtOrganizationRequest request);
        public Task<string> RemoveOrganization(int id);
        public Task<string> AddCourt(Court request);
        public Task<string> EditCourt(Court request);
        public Task<string> RemoveCourt(int id);
        public Task<GerCourtOrganizationsResponse> GetCourtOrganizations(GetCourtOranizationRequest request);
        public Task<GetCourtOrganizationByIdResponse> GetCourtOrganizationById(int id);
        public Task<GetCourtByIdResponse> GetCourtById(int id);
        public Task<ResponseBase> AddSchedule(List<AddScheduleRequest> request);
        public Task<GetAllScheduleResponse> GetAllSchedule();
        public Task<GetAvailableTimeSlotsResponse> GetAvailableTimeSlots(GetAvailableTimeSlotsRequest request);
    }
}