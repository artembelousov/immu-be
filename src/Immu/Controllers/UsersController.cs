using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Immu.Data;
using Immu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Immu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly Manager _manager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(Manager manager, ILogger<UsersController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [HttpGet]
        [Route("{email}")]
        public async Task<ActionResult> GetUser([FromRoute]string email)
        {
            try
            {
                await _manager.UpdateUserScoreAsync(email);
                var user = await _manager.GetUserAsync(email);

                return new OkObjectResult(user);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody]User user)
        {
            try
            {
                var createdUser = await _manager.CreateUserAsync(user);
                return new CreatedResult("", createdUser);
            }
            catch (DuplicateNameException)
            {
                return new ConflictObjectResult("User with this email already exists");
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpPut]
        [Route("{email}")]
        public async Task<ActionResult> UpdateUser([FromRoute]string email, [FromBody]User user)
        {
            if (email != user.Email)
            {
                return new BadRequestObjectResult("Email in Body does not correspond the Email in route");
            }

            try
            {
                await _manager.UpdateUserAsync(user);
                return new OkResult();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpDelete]
        [Route("{email}")]
        public async Task<ActionResult> DeleteUser([FromRoute]string email)
        {
            try
            {
                await _manager.DeleteUserAsync(email);
                return new NoContentResult();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpPost]
        [Route("{email}/lefthome")]
        public async Task<ActionResult> ReportLeftHome([FromRoute]string email)
        {
            try
            {
                await _manager.LeaveHomeAsync(email);
                return new NoContentResult();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpPost]
        [Route("{email}/camehome")]
        public async Task<ActionResult> ReportCameHome([FromRoute]string email)
        {
            try
            {
                await _manager.ComeHomeAsync(email);
                return new NoContentResult();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpPut]
        [Route("{email}/challenges/{challengeId}")]
        public async Task<ActionResult> UpdateUserChallenge([FromRoute]string email, [FromRoute]Int64 challengeId, [FromBody] UserChallengeUpdateRequest body)
        {
            try
            {
                var result = await _manager.SetUserChallengeStatus(email, challengeId, body.Status);
                return new OkObjectResult(result);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpGet]
        [Route("{email}/challenges")]
        public async Task<ActionResult> GetUserChallenges([FromRoute]string email, [Required][FromQuery] ChallengeCategory category)
        {
            try
            {
                var challenges = await _manager.GetUserChallengesAsync(email, category);
                return new OkObjectResult(challenges);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }
    }
}
