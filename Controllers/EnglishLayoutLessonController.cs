using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EnglishLayoutLessonController : ControllerBase
    {
        private EnglishLayoutLessonModel _model;
        private ILogger _logger;
        public EnglishLayoutLessonController(TypingTutorDbContext context, ILogger<EnglishLayoutLessonController> logger)
        {
            _logger = logger;
            _model = new (context);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnglishLayoutLesson>>> GetLessons()
        {
            var lessonsCollection = await _model.GetAllLessonsAsync();
            if (lessonsCollection is null)
            {
                _logger.LogWarning("GetLessons method return NULL!");
                return NotFound();
            }
              
            _logger.LogInformation("GetLessons method return collection!");
            return Ok(lessonsCollection);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EnglishLayoutLesson>>> GetLessonsByLevelId(int id)
        {
            var lessonsCollection = await _model.GetLessonsByLevelIdAsync(id);
            if (lessonsCollection is null)
                return NotFound();
            _logger.LogInformation($"[{nameof(GetLessonsByLevelId)}] method is success!");
            return Ok(lessonsCollection);
        }
        [HttpPut("{id}/lesson")]
        public async Task<IActionResult> UpdateLesson(int id, EnglishLayoutLesson lesson)
        {
            int code = await _model.UpdateLessonAsync(id, lesson);
            if (code == 0)
                return BadRequest();
            return NoContent();
        }

    }
}
