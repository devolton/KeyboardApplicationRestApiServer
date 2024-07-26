
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using KeyboardApplicationRestApiServer.Shared.Tools;
using System.Collections;
using System.Xml;
using KeyboardApplicationRestApiServer.Database.Entities;
using KeyboardApplicationRestApiServer.Database.Context;

namespace CourseProjectKeyboardApplication.Database.Models
{
    public class UserModel:BaseTypingTutorModel
    {
        private DbSet<User> _users;

        public UserModel(TypingTutorDbContext context):base(context)
        { 
            _users = _context.Users;
        }

        public async Task<User?> GetUserByIdAsync(int Id)
        {
            return await _users.Include(u => u.TypingTestResults)
                                 .Include(u => u.EnglishLayoutLevel)
                                 .Include(u => u.EnglishLayoutLesson)
                                 .FirstOrDefaultAsync(oneUser => oneUser.Id.Equals(Id));
        }
        public async Task<bool> IsUniqueEmailAsync(string email)
        {
            return ! await _users.AnyAsync(oneUser => oneUser.Email.Equals(email));
        }
        public async Task<bool> IsUniqueLoginAsync(string login)
        {
            return ! await _users.AnyAsync(oneUser => oneUser.Login.Equals(login));
        }
        public async Task<int> UpdateUserAsync(User user)
        {
            int successOperationCode = 0;
            User? userInDbSet = await GetUserByIdAsync(user.Id);
            if (userInDbSet != null)
            {
                userInDbSet.Email = user.Email;
                userInDbSet.Name = user.Name;
                userInDbSet.Login = user.Login;
                userInDbSet.Password =user.Password;
                userInDbSet.EnglishLayoutLesson = user.EnglishLayoutLesson;
                userInDbSet.EnglishLayoutLevel = user.EnglishLayoutLevel;
                userInDbSet.EnglishLayoutLessonId = user.EnglishLayoutLessonId;
                userInDbSet.EnglishLayoutLevelId = user.EnglishLayoutLevelId;
                userInDbSet.AvatarPath = user.AvatarPath;
                successOperationCode++;
            }
            return successOperationCode;

        }
        public async Task<int> RemoveUserAsync(int id)
        {
            int successOperationCode = 0;
            User? user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _users.Remove(user);
                successOperationCode++;
            }
            return successOperationCode;

        }
        public async Task<int> AddNewUserAsync(User newUser) // maybe change into int 
        {
            int successCode = 0;
            await Task.Run(() =>
            {
                try
                {
                    _users.Add(newUser);
                    successCode++;
                }
                catch
                {
                  
                }
            });
            return successCode;
        }
        public async Task<bool> IsUserExistByEmailAsync(string email)
        {
            return await _users.AnyAsync(oneUser => oneUser.Email == email);
        }
        public async Task<bool> IsUserExistByLoginAsync(string login)
        {
            return await _users.AnyAsync(oneUser => oneUser.Login == login);
        }
        public async Task<User?> GetUserByLoginOrEmailAndPasswordAsync(string loginOrEmail, string shaPassword)
        {
            return await _users.Include(u => u.TypingTestResults)
                                 .Include(u => u.EnglishLayoutLevel)
                                 .Include(u => u.EnglishLayoutLesson)
                                 .FirstOrDefaultAsync(oneUser => (oneUser.Login == loginOrEmail || oneUser.Email == loginOrEmail)
            && oneUser.Password == shaPassword);
        }
        public async Task<bool> IsUserExistById(int id)
        {
            return await _users.AnyAsync(user => user.Id == id);
        }

      
    }
}
