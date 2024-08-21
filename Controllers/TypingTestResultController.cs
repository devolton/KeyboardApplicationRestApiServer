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
                _logger.LogWarning($"[{nameof(GetUserTests)}] return Null! Date: "+ DateTime.Now);
                return NotFound();
            }
            _logger.LogInformation($"[{nameof(GetUserTests)}] return collection. Count: "+testsCollection.Count().ToString() + "Date: "+DateTime.Now);
            return Ok(testsCollection);
        }
        [Authorize]
        [HttpGet("BestUserTest/{userId}")]
        public async Task<ActionResult<TypingTestResult?>> GetBestUserTest(int userId)
        {
            var bestUserTest= await _model.GetBestUserTestResultAsync(userId);
            if(bestUserTest is null)
            {
                _logger.LogWarning("GetBestUserTest return NULL! Date: "+DateTime.Now);
                return NotFound();
            }
            _logger.LogInformation("GetBestUserTest return bestUserTest! Date: "+DateTime.Now);
            return Ok(bestUserTest);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypingTestResult(int id)
        {
            var removedCount = await _model.RemoveUsersTestAsync(id);
            if (removedCount == 0)
            {
                _logger.LogWarning("DeleteTypingTestResult method error!");
                return NotFound();
            }
            _logger.LogInformation("DeleteTypingTestResult method success!");
            return Ok();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewTypingTestResult(TypingTestResult typingTestResult)
        {
            var code = await _model.AddNewTypingTestResultAsync(typingTestResult);
            if (code == 0)
            {
                _logger.LogWarning("AddNewTypingTest method error!");
                return BadRequest();
            }
            _logger.LogInformation("AddNewTypingTest is successfully!");
            return Ok();
        }
        [Authorize]
        [HttpPost("AddRange")]
        public async Task<IActionResult> AddRangeTypingTestResult(IEnumerable<TypingTestResult> typingTestResultCollection)
        {
            var code = await _model.AddRangeTypingTestResultsAsync(typingTestResultCollection, _logger);
            if(code == 0)
            {
                _logger.LogError($"[{nameof(AddRangeTypingTestResult)}] method error!");
                return BadRequest();
            }
            _logger.LogInformation($"[{nameof(AddRangeTypingTestResult)}] method is success. Added count: {typingTestResultCollection.Count()}. Time: {DateTime.Now}");
            return Ok();
        }
    }
}
