using Models.Models.Review;

namespace Interfaces.Interfaces
{
    public interface IReviewService
    {
        public Task<ReviewUserResponse> ReviewUser(ReviewUserRequest request);
        public Task<ReviewCourtResponse> ReviewCourt(ReviewCourtRequest request);
        public Task<SetGameResultsResponse> SetGameResults(SetGameResultsRequest request);
    }
}
