using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EnglishLayoutLevelController : ControllerBase
    {
        private EnglishLayoutLevelModel _model;
        private ILogger _logger;
        public EnglishLayoutLevelController(TypingTutorDbContext context, ILogger<EnglishLayoutLevelController> logger)
        {
            _logger = logger;
            _model = new EnglishLayoutLevelModel(context);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnglishLayoutLevel>>> GetLevel()
        {
            var collection = await _model.GetLevelsAsync();
            if (collection is null)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetLevel)}] method return NULL!"); 
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetLevel)}] method return collectoin of {collection.Count()} EnglishLayoutLevel elements!");
            return Ok(collection);
        }
        [HttpPost]
        public async Task<IActionResult> AddLevel(EnglishLayoutLevel newLevel)
        {
            int code = await _model.AddLevelAsync(newLevel,_logger);
            if (code == 0)
            {
                _logger.LogError($"{DateTime.Now} - [{nameof(AddLevel)}] method error!");
                return BadRequest();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(AddLevel)}] method successful!");
            return NoContent();
        }
        [HttpPut("{id}/level")]
        public async Task<IActionResult> UpdateLevel(int id, EnglishLayoutLevel level)
        {
            var code = await _model.UpdateLevel(id, level,_logger);
            if (code == 0)
            {
                _logger.LogError($"{DateTime.Now} - [{nameof(UpdateLevel)}] method error!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(UpdateLevel)}] method successful!");
            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EnglishLayoutLevel>> GetLevelById(int id)
        {
            var level = await _model.GetLevelByIdAsync(id);
            if (level is null)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetLevelById)}] method return null!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetLevelById)}] method is successful!");
            return Ok(level);
        }
    }
}
