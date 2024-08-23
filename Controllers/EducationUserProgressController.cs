using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EducationUserProgressController : ControllerBase
    {
        private ILogger _logger;
        private EducationUserProgressModel _model;
        public EducationUserProgressController(TypingTutorDbContext context, ILogger<EducationUserProgressController> logger)
        {
            _logger = logger;
            _model = new(context);

        }
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<EducationUsersProgress>>> GetEducationProgressesByUserId(int userId)
        {
            var collection = await _model.GetUsersEducationProgressAsync(userId);
            if (collection is null)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetEducationProgressesByUserId)}] method return null!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetEducationProgressesByUserId)}] method return collection. Elements count: {collection.Count()}");
            return Ok(collection);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddEducationProgress(EducationUsersProgress educationUserProgress)
        {
            await _model.AddNewEducationUserProgressAsync(educationUserProgress, _logger);
            return NoContent();
        }
        [Authorize]
        [HttpPost("AddRange")]
        public async Task<IActionResult> AddRangeEducationProgress(IEnumerable<EducationUsersProgress> educationUsersProgressCollection)
        {
           
            await _model.AddRangeNewEducationProgressAsync(educationUsersProgressCollection, _logger);
            return NoContent();
        }
        [Authorize]
        [HttpPut("UpdateRange")]
        public async Task<IActionResult> UpdateRangeEducatonProgress(IEnumerable<EducationUsersProgress> educationUsersProgressCollection)
        {
            await _model.UpdateRangeEducationProgressAsync(educationUsersProgressCollection, _logger);
            return NoContent();
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateEducationProgress(EducationUsersProgress educationProgress)
        {
            await _model.UpdateEducationProgressAsync(educationProgress,_logger);
            return NoContent();

        }

    }
}
