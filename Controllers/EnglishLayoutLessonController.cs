using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Authorization;
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
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetLessons)}] method return NULL!");
                return NotFound();
            }
              
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetLessons)}] method return collection of {lessonsCollection.Count()} EnglishLayoutLesson elements!");
            return Ok(lessonsCollection);

        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<EnglishLayoutLesson>>> GetLessonsByLevelId(int id)
        {
            var lessonsCollection = await _model.GetLessonsByLevelIdAsync(id);
            if (lessonsCollection is null)
            {
                _logger.LogInformation($"{DateTime.Now} - [{nameof(GetLessonsByLevelId)}] method return NULL!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetLessonsByLevelId)} method return collecton of {lessonsCollection.Count()} EngilshLayoutLesson elements!");
            return Ok(lessonsCollection);
        }
        [HttpPut("{id}/lesson")]
        public async Task<IActionResult> UpdateLesson(int id, EnglishLayoutLesson lesson)
        {
            int code = await _model.UpdateLessonAsync(id, lesson, _logger);
            if (code == 0)
            {
                _logger.LogError($"{DateTime.Now} - [{nameof(UpdateLesson)}] method error!");
                return BadRequest();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(UpdateLesson)}] method success!");
            return NoContent();
        }

    }
}
