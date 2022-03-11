using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/chats")]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            return Ok(await _chatService.GetMyChats(this.GetUserId()));
        }

        [HttpGet("{user_id}")]
        public async Task<IActionResult> GetChatWithUser([FromRoute] int user_id)
        {
            var result = await _chatService.GetMyChatWithUser(this.GetUserId(), user_id);
            if (result == null)
            {
                return BadRequest(new ErrorResponse("Bad request!"));
            }
            return Ok(result);
        }

        [HttpPost("{user_id}")]
        public async Task<IActionResult> Send([FromRoute] int user_id, [FromBody] MessageModel model)
        {
            bool result = await _chatService.Send(this.GetUserId(), user_id, model.Message);
            if (!result)
            {
                return BadRequest(new ErrorResponse("Bad request!"));
            }
            return Ok(new MessageModel("successfull"));
        }
    }
}
