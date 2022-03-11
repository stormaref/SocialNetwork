using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v1/connection_requests")]
    [ApiController]
    public class ConnectionsController : ControllerBase
    {
        private readonly IConnectionService _connectionService;

        public ConnectionsController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] GroupIdRequest request)
        {
            var result = await _connectionService.Send(this.GetUserId(), request.GroupId);
            if (result)
            {
                return Ok(new MessageModel("successfull"));
            }
            return BadRequest(new ErrorResponse("Bad request!"));
        }

        [HttpGet]
        public async Task<IActionResult> GetMyGroupConnectionRequests()
        {
            var result = await _connectionService.GetMyGroupConnectionRequests(this.GetUserId());
            if (result == null)
            {
                return BadRequest(new ErrorResponse("Bad request!"));
            }

            return Ok(result);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> GetMyGroupConnectionRequests([FromBody] GroupIdRequest request)
        {
            var result = await _connectionService.Accept(this.GetUserId(), request.GroupId);
            if (result)
            {
                return Ok(new MessageModel("successfull"));
            }
            return BadRequest(new ErrorResponse("Bad request!"));
        }
    }
}
