using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TypingTestResultController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TypingTestResultModel _model;
        public TypingTestResultController(TypingTutorDbContext context,ILogger<TypingTestResultController> logger )
        {
            _logger = logger;
            _model = new TypingTestResultModel(context);
        }
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<TypingTestResult>>> GetUserTests(int userId)
        {
            
            var testsCollection =await _model.GetTypingTestResultsByUserIdAsync(userId);
            if (testsCollection is null)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetUserTests)}] return Null!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetUserTests)}] return collecton of {testsCollection.Count()} TypingTestResult elements!");
            return Ok(testsCollection);
        }
        [Authorize]
        [HttpGet("BestUserTest/{userId}")]
        public async Task<ActionResult<TypingTestResult?>> GetBestUserTest(int userId)
        {
            var bestUserTest= await _model.GetBestUserTestResultAsync(userId);
            if(bestUserTest is null)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(GetBestUserTest)}] method return NULL!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(GetBestUserTest)}] method return {nameof(TypingTestResult)} entity!");
            return Ok(bestUserTest);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypingTestResult(int id)
        {
            var removedCount = await _model.RemoveUsersTestAsync(id, _logger);
            if (removedCount == 0)
            {
                _logger.LogWarning($"{DateTime.Now} - [{nameof(DeleteTypingTestResult)}] method error!");
                return NotFound();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(DeleteTypingTestResult)}] method successful!");
            return Ok();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewTypingTestResult(TypingTestResult typingTestResult)
        {
            var code = await _model.AddNewTypingTestResultAsync(typingTestResult, _logger);
            if (code == 0)
            {
                _logger.LogError($"{DateTime.Now} - [{nameof(AddNewTypingTestResult)}] method error!");
                return BadRequest();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(AddNewTypingTestResult)}] method is successfully!");
            return Ok();
        }
        [Authorize]
        [HttpPost("AddRange")]
        public async Task<IActionResult> AddRangeTypingTestResult(IEnumerable<TypingTestResult> typingTestResultCollection)
        {
            var code = await _model.AddRangeTypingTestResultsAsync(typingTestResultCollection, _logger);
            if(code == 0)
            {
                _logger.LogError($"{DateTime.Now} - [{nameof(AddRangeTypingTestResult)}] method error!");
                return BadRequest();
            }
            _logger.LogInformation($"{DateTime.Now} - [{nameof(AddRangeTypingTestResult)}] method is success. Added count: {typingTestResultCollection.Count()}");
            return Ok();
        }
    }
}
