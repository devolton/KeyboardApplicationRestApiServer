using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EducationUserProgressController : ControllerBase
    {
        private ILogger _logger;
        private EducationUserProgressModel _model;
        public EducationUserProgressController(TypingTutorDbContext context, ILogger <EducationUserProgressController> logger)
        {
            _logger = logger;
            _model = new(context);
            
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<EducationUsersProgress>>> GetEducationProgressesByUserId(int userId)
        {
            var collection = await _model.GetUsersEducationProgressAsync(userId);
            if(collection is null)
                return NotFound();
            return Ok(collection);
        }
        [HttpPost]
        public async Task<IActionResult> AddEducationProgress(EducationUsersProgress educationUserProgress)
        {
            await _model.AddNewEducationUserProgressAsync(educationUserProgress);
            return NoContent();
        }
        [HttpPost("AddRange")]
        public async Task<IActionResult> AddRangeEducationProgress(IEnumerable<EducationUsersProgress> educationUsersProgresses)
        {
            await _model.AddRangeNewEducationProgressAsync(educationUsersProgresses);
            return NoContent();
        }
    }
}
