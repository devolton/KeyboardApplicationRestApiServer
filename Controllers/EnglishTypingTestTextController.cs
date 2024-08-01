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
                return NotFound();
            return Ok(text);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnglishTypingTestText>>> GetAllTexts()
        {
            var collection = await _model.GetAllTextsAsync();
            if (collection is null)
                return NotFound();
            return Ok(collection);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTextById(int id)
        {
            var successCode = await _model.RemoveTextByIdAsync(id);
            if (successCode == 0)
                return NotFound();
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewText(EnglishTypingTestText text)
        {
            int successCode = await _model.AddNewTextAsync(text);
            if (successCode == 0)
            {
                _logger.LogWarning("AddNewText ERROR!");
                return BadRequest();
            }
            _logger.LogInformation("AddNewText success!");
            return NoContent();
        }
    }
}
