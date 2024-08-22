using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EnglishTypingTestTextController : ControllerBase
    {
        private EnglishTypingTestTextModel _model;
        private ILogger _logger;
        public EnglishTypingTestTextController(TypingTutorDbContext context, ILogger<EnglishTypingTestTextController> logger)
        {
            _logger = logger;
            _model = new(context);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EnglishTypingTestText>> GetTextById(int id)
        {
            var text = await _model.GetTextByIdAsync(id);
            if (text is null)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetTextById)}] method return NotFound!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetTextById)}] method return EnglishTypingTestText element!");
            return Ok(text);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnglishTypingTestText>>> GetAllTexts()
        {
            var collection = await _model.GetAllTextsAsync();
            if (collection is null)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetAllTexts)}] method return NULL");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetAllTexts)}] method return collection of {collection.Count()} EnglishTypingTestText elements!");
            return Ok(collection);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTextById(int id)
        {
            var successCode = await _model.RemoveTextByIdAsync(id, _logger);
            if (successCode == 0) {
                _logger.LogError($"{DateTime.Now} - [{nameof(DeleteTextById)}] method error!");
                return NotFound();
            }
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewText(EnglishTypingTestText text)
        {
            int successCode = await _model.AddNewTextAsync(text, _logger);
            if (successCode == 0)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(AddNewText)}] method error!");
                return BadRequest();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(AddNewText)}] method successful!");
            return NoContent();
        }
    }
}
