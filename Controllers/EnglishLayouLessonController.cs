using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EnglishLayouLessonController : ControllerBase
    {
        private EnglishLayoutLessonModel _model;
        private ILogger _logger;
        public EnglishLayouLessonController(TypingTutorDbContext context, ILogger<EnglishLayouLessonController> logger)
        {
            _logger = logger;
            _model = new (context);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EnglishLayoutLesson>>> GetLessonsByLevelId(int id)
        {
            var lessonsCollection = await _model.GetLessonsByLevelIdAsync(id);
            if (lessonsCollection is null)
                return NotFound();
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
