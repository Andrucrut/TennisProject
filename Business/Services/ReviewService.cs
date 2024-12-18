using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Models.Review;
using Newtonsoft.Json;

namespace Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly TennisDbContext tennisDb;
        public ReviewService(TennisDbContext tennisDb)
        {
            this.tennisDb = tennisDb;
        }
        public async Task<ReviewCourtResponse> ReviewCourt(ReviewCourtRequest request)
        {
            try
            {
                await tennisDb.CourtReviews.AddAsync(new CourtReview
                {
                    ReviewerId = request.ReviewerId,
                    CourtOrganizationId = request.CourtOrganizationId,
                    GameId = request.GameId,
                    Disappointments = request.Disappointments,
                    Satisfactions = request.Satisfactions,
                    Comment = request.Comment,
                    Rating = request.Rating,
                    CreatedAt = DateTime.UtcNow
                });

                await tennisDb.Logs.AddAsync(new Log
                {
                    GameId = request.GameId,
                    UserId = request.ReviewerId,
                    Request = JsonConvert.SerializeObject(request).ToString(),
                    LogLevel = LogLevel.Info,
                    Time = DateTime.UtcNow
                });
                await tennisDb.SaveChangesAsync();
                return new ReviewCourtResponse { Success = true };
            }
            catch(Exception ex)
            {
                await tennisDb.Logs.AddAsync(new Log
                {
                    GameId = request.GameId,
                    UserId = request.ReviewerId,
                    Request = JsonConvert.SerializeObject(request).ToString(),
                    LogLevel = LogLevel.Error,
                    Time = DateTime.UtcNow,
                    Message = ex.Message,
                });
                await tennisDb.SaveChangesAsync();
                return new ReviewCourtResponse { Success = false, ExceptionMess = ex.Message };
            }

        }

        public async Task<SetGameResultsResponse> SetGameResults(SetGameResultsRequest request)
        {
            try
            {
                var gameResult = new GameResult
                {
                    GameId = request.GameId,
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                foreach (var winnerId in request.WinnerIds)
                {
                    var user = await tennisDb.Users.FindAsync(winnerId);
                    if (user != null)
                        gameResult.Winners.Add(user);
                }

                if(request.ResultsOfSets != null)
                {
                    var opponentGameOrder = await tennisDb.GameOrders.FirstOrDefaultAsync(_ => _.GameId == request.GameId && _.UserId != request.UserId && _.Status == GameOrderStatus.Played);
                    foreach (var set in request.ResultsOfSets)
                    {

                        var score = new GameScore
                        {
                            CreatorId = request.UserId,
                            SetNumber = set.SetNumber,
                            CreatorScore = set.MyScore,
                            OpponentScore = set.OpponentScore,
                            GameId = request.GameId,
                            OpponentId = opponentGameOrder.UserId
                        };
                        gameResult.ScoreResults.Add(score);
                    }
                }

                await tennisDb.Logs.AddAsync(new Log
                {
                    GameId = request.GameId,
                    UserId = request.UserId,
                    Request = JsonConvert.SerializeObject(request).ToString(),
                    Response = JsonConvert.SerializeObject(gameResult).ToString(),
                    Controller = "Review",
                    Service = "SetGameResults",
                    LogLevel = LogLevel.Info,
                    Time = DateTime.UtcNow
                });

                await tennisDb.GameResults.AddAsync(gameResult);
                await tennisDb.SaveChangesAsync();

                return new SetGameResultsResponse { Success = true };
            }
            catch (Exception ex)
            {
                await tennisDb.Logs.AddAsync(new Log
                {
                    GameId = request.GameId,
                    UserId = request.UserId,
                    Request = JsonConvert.SerializeObject(request).ToString(),
                    Controller = "Review",
                    Service = "SetGameResults",
                    LogLevel = LogLevel.Error,
                    Time = DateTime.UtcNow
                });
                return new SetGameResultsResponse { Success = false, ExceptionMess = ex.Message};
            }

        }

        public async Task<ReviewUserResponse> ReviewUser(ReviewUserRequest request)
        {
            try
            {
                await tennisDb.UserReviews.AddAsync(new UserReview
                {
                    GameId = request.GameId,
                    ReviewerId = request.ReviewerId,
                    ReviewedPlayerId = request.ReviewedPlayerId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    CreatedAt = DateTime.UtcNow,
                });
                await tennisDb.Logs.AddAsync(new Log
                {
                    GameId = request.GameId,
                    UserId = request.ReviewerId,
                    Request = JsonConvert.SerializeObject(request).ToString(),
                    LogLevel = LogLevel.Info,
                    Time = DateTime.UtcNow
                });
                await tennisDb.SaveChangesAsync();
                return new ReviewUserResponse { Success = true };
            }
            catch (Exception ex)
            {
                await tennisDb.Logs.AddAsync(new Log
                {
                    GameId = request.GameId,
                    UserId = request.ReviewerId,
                    Request = JsonConvert.SerializeObject(request).ToString(),
                    LogLevel = LogLevel.Error,
                    Time = DateTime.UtcNow,
                    Message = ex.Message,
                });
                await tennisDb.SaveChangesAsync();
                return new ReviewUserResponse { Success = false, ExceptionMess = ex.Message };

            }
        }
    }
}
