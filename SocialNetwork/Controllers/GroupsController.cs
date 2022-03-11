using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IAuthService _authService;

        public GroupsController(IGroupService groupService, IAuthService authService)
        {
            _groupService = groupService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            return Ok(await _groupService.All());
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetUserGroup()
        {
            var userId = this.GetUserId();
            var result = await _groupService.GetUserGroup(userId);
            if (result == null)
            {
                return BadRequest(new ErrorResponse("Bad request!"));
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var userId = this.GetUserId();
            var has = await _groupService.CheckUserGroup(userId);
            if (has)
            {
                return BadRequest(new ErrorResponse("Bad request!"));
            }
            var user = await _authService.GetUser(userId);
            var group = await _groupService.CreateGroup(user, request.Name, request.Description);
            return Ok(new CreateGroupResponse(group.Id, "successfull"));
        }
    }

    public class CreateGroupRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateGroupResponse
    {
        public CreateGroupResponse(int id, string message)
        {
            Group = new GroupResponse(id);
            Message = message;
        }
        public GroupResponse Group { get; set; }
        public string Message { get; set; }
    }

    public class GroupResponse
    {
        public int Id { get; set; }

        public GroupResponse(int id)
        {
            Id = id;
        }
    }
}
