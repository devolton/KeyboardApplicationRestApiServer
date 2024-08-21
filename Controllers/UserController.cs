using CourseProjectKeyboardApplication.Database.Models;
using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KeyboardApplicationRestApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserModel _model;

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;


        public UserController(IConfiguration configuration,TypingTutorDbContext context,ILogger<UserController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _model = new UserModel(context);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User?>> GetUserById(int id)
        {
            var user = await _model.GetUserByIdAsync(id);
            if (user is null)
            {
                _logger.LogWarning($"[{nameof(GetUserById)}] method return NULL!");
                return NotFound();
            }
            _logger.LogInformation($"[{nameof(GetUserById)}] method return user(Id: {user.Id})");
            return Ok(user);
        }



        // POST <UserController>
        [HttpPost]
        public async Task<ActionResult<KeyValuePair<User,string?>>> Post(User newUser)
        {
            var addedUser = await _model.AddNewUserAsync(newUser,_logger);
            if(addedUser is null)
            {
                _logger.LogWarning("[UserPost] method ERROR!");
                return BadRequest();
            }
            _logger.LogInformation("[UserPost] method: user adding operation is successful!");
            var token = GenerateJwtToken(addedUser);
            return Ok(new KeyValuePair<User,string?>(addedUser,token));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            bool isUserExist = await _model.IsUserExistById(id);
            if (!isUserExist)
            {
                _logger.LogWarning($"[{nameof(UpdateUser)}] method return Null!");
                return NotFound();
            }
            var successCode = await _model.UpdateUserAsync(user);
            if (successCode == 0)
            {
                _logger.LogWarning($"[{nameof(UpdateUser)}] method return Null!");
                return BadRequest();
            }
            _logger.LogInformation($"[{nameof(UpdateUser)}] is success!");
            return Ok();

        }

        [Authorize]
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
        public async Task<ActionResult<KeyValuePair<User,string>>> GetUserByLoginOrEmailAndPassword(string loginOrEmail, string shaPassword)
        {
            var user = await _model.GetUserByLoginOrEmailAndPasswordAsync(loginOrEmail, shaPassword);

            if (user is null)
            {
                _logger.LogWarning($"[{nameof(GetUserByLoginOrEmailAndPassword)}] method return NULL!");
                return NotFound();
            }
            _logger.LogInformation($"[{nameof(GetUserByLoginOrEmailAndPassword)}] method return User(ID: {user.Id}");
            var token = GenerateJwtToken(user);
            return Ok(new KeyValuePair<User,string>(user,token));
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Login)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
