using Interfaces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Review;

namespace TennisProject.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private IReviewService reviewService { get; set; }
        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [AllowAnonymous]
        [HttpPost("ReviewUser")]
        public async Task<ReviewUserResponse> ReviewUser(ReviewUserRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ReviewUserResponse { Success = false, Message = "Validation error" };

            return await reviewService.ReviewUser(request);
        }

        [AllowAnonymous]
        [HttpPost("ReviewCourt")]
        public async Task<ReviewCourtResponse> ReviewCourt(ReviewCourtRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new ReviewCourtResponse { Success = false, Message = "Validation error" };

            return await reviewService.ReviewCourt(request);
        }

        [AllowAnonymous]
        [HttpPost("SetGameResults")]
        public async Task<SetGameResultsResponse> SetGameResults(SetGameResultsRequest request, [FromServices] InitDataValidator initDataValidator)
        {
            var initData = Request.Headers["InitData"];
            var validatorResponse = await initDataValidator.Validate(initData);
            if (validatorResponse.IsValidated == false)
                return new SetGameResultsResponse { Success = false, Message = "Validation error" };

            return await reviewService.SetGameResults(request);
        }
    }
}
