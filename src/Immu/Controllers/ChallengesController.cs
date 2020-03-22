using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Immu.Data;
using Immu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Immu.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChallengesController : ControllerBase
    {
        private readonly Manager _manager;
        private readonly ILogger<UsersController> _logger;

        public ChallengesController(Manager manager, ILogger<UsersController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetChallenges([FromQuery]ChallengeCategory category = ChallengeCategory.Unknown)
        {
            try
            {
                var challenges = await _manager.GetChallengesAsync(category);
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

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetChallenge([FromRoute]Int64 id)
        {
            try
            {
                var challenge = await _manager.GetChallengeAsync(id);
                return new OkObjectResult(challenge);
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
        public async Task<ActionResult> CreateChallenge([FromBody]Challenge challenge)
        {
            try
            {
                var createdChallenge = await _manager.CreateChallengeAsync(challenge);
                return new CreatedResult("", createdChallenge);
            }
            catch (DuplicateNameException)
            {
                return new ConflictObjectResult("Challenge already exists");
            }
            catch (Exception e)
            {
                return new ObjectResult($"Unexpected exception during execution: {e.Message}") { StatusCode = 500 };
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteChallenge([FromRoute]Int64 id)
        {
            try
            {
                await _manager.DeleteChallegeAsync(id);
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
    }
}
