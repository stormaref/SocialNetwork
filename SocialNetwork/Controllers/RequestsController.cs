using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/join_requests")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserRequests()
        {
            return Ok(await _requestService.GetUserRequests(this.GetUserId()));
        }

        [HttpGet("group")]
        public async Task<IActionResult> GetGroupRequests()
        {
            var result = await _requestService.GetGroupRequests(this.GetUserId());
            if (result == null)
            {
                return BadRequest(new ErrorResponse("Bad request!"));
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] GroupIdRequest request)
        {
            var result = await _requestService.Request(this.GetUserId(), request.GroupId);
            if (result)
            {
                return Ok(new MessageModel("successfull"));
            }
            return BadRequest(new ErrorResponse("Bad request!"));
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Accept([FromBody] AcceptJoinRequest request)
        {
            var result = await _requestService.Accept(this.GetUserId(), request.JoinRequestId);
            if (result)
            {
                return Ok(new MessageModel("successfull"));
            }
            return BadRequest(new ErrorResponse("Bad request!"));
        }

        public class AcceptJoinRequest
        {
            public int JoinRequestId { get; set; }
        }
    }
}
