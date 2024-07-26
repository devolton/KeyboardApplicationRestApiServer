using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserModel _model;

        // create usersEntitiesCollection
        private readonly ILogger _logger;


        public UserController(TypingTutorDbContext context,ILogger<UserController> logger)
        {
            _logger = logger;
            _model = new UserModel(context);
        }

        // GET: User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User?>> GetUserById(int id)
        {
            var user = await _model.GetUserByIdAsync(id);
            if (user is null)
            {
                //add logger
                return NotFound();
            }
            _logger.Log(LogLevel.Information, user.ToString());
            return Ok(user);
        }



        // POST <UserController>
        [HttpPost]
        public async Task<IActionResult> Post(User newUser)
        {
            var successCode = await _model.AddNewUserAsync(newUser);
            if(successCode == 0)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // PUT <UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            bool isUserExist = await _model.IsUserExistById(id);
            if (!isUserExist)
            {
                return BadRequest();
            }
            var successCode = await _model.UpdateUserAsync(user);
            if (successCode == 0)
            {
                return NotFound();
            }
            return Ok();

        }

        // DELETE <UserController>/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var successCode = await _model.RemoveUserAsync(id);
            if (successCode == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
        // GET: Users/IsUniqueEmail
        [HttpGet("IsUniqueEmail/{email}")]
        public async Task<ActionResult<bool>> IsUniqueEmail(string email)
        {
            var isUnique = await _model.IsUniqueEmailAsync(email);
            return Ok(isUnique);
        }

        // GET: Users/IsUniqueLogin
        [HttpGet("IsUniqueLogin/{login}")]
        public async Task<ActionResult<bool>> IsUniqueLogin(string login)
        {
            var isUnique = await _model.IsUniqueLoginAsync(login);
            return Ok(isUnique);
        }
        //Get: User/IsUserExistByEmail
        [HttpGet("IsUserExistByEmail/{email}")]
        public async Task<ActionResult<bool>> IsUserExistByEmail(string email)
        {
            var isExist = await _model.IsUserExistByEmailAsync(email);
            return Ok(isExist);
        }
        [HttpGet("IsUserExistByLogin/{login}")]
        public async Task<ActionResult<bool>> IsUserExistByLogin(string login)
        {
            var isExist = await _model.IsUserExistByLoginAsync(login);
            return Ok(isExist);
        }
        // GET:User/LoginOrEmailAndPassword?loginOrEmail=value&shaPassword=value
        [HttpGet("LoginOrEmailAndPassword")]
        public async Task<ActionResult<User>> GetUserByLoginOrEmailAndPassword(string loginOrEmail, string shaPassword)
        {
            var user = await _model.GetUserByLoginOrEmailAndPasswordAsync(loginOrEmail, shaPassword);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
