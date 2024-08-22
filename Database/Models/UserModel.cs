
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
using Microsoft.Extensions.Logging;

namespace CourseProjectKeyboardApplication.Database.Models
{
    public class UserModel : BaseTypingTutorModel
    {
        private DbSet<User> _users;

        public UserModel(TypingTutorDbContext context) : base(context)
        {
            _users = _context.Users;
        }

        public async Task<User?> GetUserByIdAsync(int Id)
        {
            return await _users.Include(u => u.TypingTestResults)
                                 .Include(u => u.EnglishLayoutLevel)
                                 .Include(u => u.EnglishLayoutLesson)
                                 .Include(u => u.EducationUsersProgresses)
                                 .FirstOrDefaultAsync(oneUser => oneUser.Id.Equals(Id));
        }
        public async Task<bool> IsUniqueEmailAsync(string email)
        {
            return !await _users.AnyAsync(oneUser => oneUser.Email.Equals(email));
        }
        public async Task<bool> IsUniqueLoginAsync(string login)
        {
            return !await _users.AnyAsync(oneUser => oneUser.Login.Equals(login));
        }
        public async Task<int> UpdateUserAsync(User user, ILogger logger)
        {
            int successOperationCode = 0;
            try
            {
                User? userInDbSet = await _context.Users.Include(u => u.EnglishLayoutLesson)
                                                        .Include(u => u.EnglishLayoutLevel)
                                                        .Include(u => u.EducationUsersProgresses)
                                                        .Include(u => u.TypingTestResults)
                                                        .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (userInDbSet != null)
                {
                    userInDbSet.Email = user.Email;
                    userInDbSet.Name = user.Name;
                    userInDbSet.Login = user.Login;
                    userInDbSet.Password = user.Password;
                    userInDbSet.EnglishLayoutLessonId = user.EnglishLayoutLessonId;
                    userInDbSet.EnglishLayoutLevelId = user.EnglishLayoutLevelId;
                    userInDbSet.AvatarPath = user.AvatarPath;

                    userInDbSet.EnglishLayoutLesson = await _context.EnglishLayoutLessons.FindAsync(user.EnglishLayoutLessonId);
                    userInDbSet.EnglishLayoutLevel = await _context.EnglishLayoutLevels.FindAsync(user.EnglishLayoutLevelId);

                    _context.Entry(userInDbSet).State = EntityState.Modified;
                   

                    try
                    {
                        await _context.SaveChangesAsync();
                        successOperationCode++;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"{DateTime.Now} - [{nameof(UpdateUserAsync)}] method error: {ex.Message}");
                    }
                }
                else
                {
                    
                    successOperationCode = -1;
                }
            }
            catch(NullReferenceException ex)
            {
                logger.LogError($"{DateTime.Now} - [{nameof(UpdateUserAsync)}] method error: {ex.Message}");
            }
            return successOperationCode;
        }


        public async Task<int> RemoveUserAsync(int id,ILogger logger)
        {
            int successOperationCode = 0;
            try
            {
                User? user = await GetUserByIdAsync(id);
                if (user != null)
                {
                    _users.Remove(user);
                    _context.Entry(user).State = EntityState.Deleted;
                    successOperationCode++;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{DateTime.Now} - [{nameof(RemoveUserAsync)}] method error: {ex.Message}");
            }
            return successOperationCode;

        }
        public async Task<User?> AddNewUserAsync(User newUser,ILogger logger)  
        {
           return await Task.Run(async () =>
            {
                try
                {
                    var lesson = _context.EnglishLayoutLessons.FirstOrDefault(oneLesson => oneLesson.Id == newUser.EnglishLayoutLessonId);
                    var level = _context.EnglishLayoutLevels.FirstOrDefault(oneLevel => oneLevel.Id == newUser.EnglishLayoutLevelId);
                    if (lesson is not null)
                    {
                        newUser.EnglishLayoutLesson = lesson;
                        _context.Entry(lesson).State = EntityState.Unchanged;
                    }
                    if (level is not null)
                    {
                        newUser.EnglishLayoutLevel = level;
                        _context.Entry(level).State = EntityState.Unchanged;
                    }
                    var addedUser = _users.Add(newUser);
                    
                    _context.Entry(newUser).State = EntityState.Added;
                    await SaveChangesAsync();
                    return addedUser;
                }
                catch (Exception ex)
                {
                    logger.LogError($"{DateTime.Now} - [{nameof(AddNewUserAsync)}] method error: {ex.Message}");

                }
                return null;
            });
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
