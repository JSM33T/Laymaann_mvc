using Laymaann.Entities.Dedicated.Message;
using Laymaann.Entities.Shared;
using Laymaann.Entities.ViewModels.Blog;
using Laymaann.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Laymaann.Web.Controllers.Api
{
	[Route("api/message")]
    [ApiController]
    public class MessageController : FoundationController
    {
        private readonly IMessageRepository _messageRepo;
        private readonly HttpContext _httpContext;
        public MessageController(IOptionsMonitor<LaymaannConfig> config, ILogger<FoundationController> logger,IHttpContextAccessor httpContextAccessor,IMessageRepository messageRepository) 
            : base(config, logger, httpContextAccessor)
        {
            _messageRepo = messageRepository;
            _httpContext = httpContextAccessor.HttpContext;
        }

        [HttpPost("sendfeedback")]
        #region message CONTROLLER
        public async Task<IActionResult> SendMessage(AddFeedbackMessage feedbackMessageRequest)
        {
            return await ExecuteActionAsync(async () =>
            {
                int statCode = StatusCodes.Status400BadRequest;
                List<BlogPost> blogPosts = [];
                List<string> errors = [];
                string Message = "";

                statCode = StatusCodes.Status200OK;

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
                var userId = userIdClaim?.Value;
                
                if(string.IsNullOrEmpty(userId))
                {
                    Message = "Unauthorized";
                    errors.Add("You are not authorized for this action");
                    statCode = StatusCodes.Status401Unauthorized;
                }
                if (string.IsNullOrEmpty(feedbackMessageRequest.Message))
                {
					Message = "Validation error";
					errors.Add("Message is required");
					statCode = StatusCodes.Status400BadRequest;
				}
				else if (feedbackMessageRequest.Message.Length <= 3)
				{
					Message = "Validation error";
					errors.Add("Message too short");
					statCode = StatusCodes.Status400BadRequest;
				}


				var feedbackMessage = new FeedbackMessage
                {
                    UserId = userId,
                    Origin = feedbackMessageRequest.Origin,
                    Message = feedbackMessageRequest.Message,
                    UserAgent = _httpContext.Request.Headers.UserAgent,
                };

                if (errors.Count == 0)
                {
                    await _messageRepo.AddFeedbackMessageAsync(feedbackMessage);
                    Message = "Feedback Sent";
                    statCode = StatusCodes.Status200OK;
                }
                
                return (statCode, 0, Message, errors);

            }, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

    }
}
