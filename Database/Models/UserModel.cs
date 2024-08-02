
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
        public async Task<int> UpdateUserAsync(User user)
        {
            int successOperationCode = 0;

            // Загружаем пользователя из базы данных
            User? userInDbSet = await _context.Users.Include(u => u.EnglishLayoutLesson)
                                                    .Include(u => u.EnglishLayoutLevel)
                                                    .Include(u => u.EducationUsersProgresses)
                                                    .Include(u => u.TypingTestResults)
                                                    .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userInDbSet != null)
            {
                // Обновляем поля
                userInDbSet.Email = user.Email;
                userInDbSet.Name = user.Name;
                userInDbSet.Login = user.Login;
                userInDbSet.Password = user.Password;
                userInDbSet.EnglishLayoutLessonId = user.EnglishLayoutLessonId;
                userInDbSet.EnglishLayoutLevelId = user.EnglishLayoutLevelId;
                userInDbSet.AvatarPath = user.AvatarPath;


                // Обновляем навигационные свойства
                userInDbSet.EnglishLayoutLesson = await _context.EnglishLayoutLessons.FindAsync(user.EnglishLayoutLessonId);
                userInDbSet.EnglishLayoutLevel = await _context.EnglishLayoutLevels.FindAsync(user.EnglishLayoutLevelId);

                // Обновляем навигационные коллекции
                userInDbSet.EducationUsersProgresses.Clear();
                foreach (var progress in user.EducationUsersProgresses)
                {
                    userInDbSet.EducationUsersProgresses.Add(await _context.EducationUsersProgresses.FindAsync(progress.Id));
                }

                userInDbSet.TypingTestResults.Clear();
                foreach (var result in user.TypingTestResults)
                {
                    userInDbSet.TypingTestResults.Add(await _context.TypingTestResults.FindAsync(result.Id));
                }

                // Устанавливаем состояние сущности
                _context.Entry(userInDbSet).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                    successOperationCode++;
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                // Если пользователь не найден, возвращаем код ошибки
                successOperationCode = -1;
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
                _context.Entry(user).State = EntityState.Deleted;
                successOperationCode++;
            }
            return successOperationCode;

        }
        public async Task<int> AddNewUserAsync(User newUser,ILogger logger)  
        {
            int successCode = 0;
            await Task.Run(async () =>
            {
                try
                {
                    //newUser.EnglishLayoutLesson = _context.EnglishLayoutLessons.FirstOrDefault(oneLesson => oneLesson.Id == newUser.EnglishLayoutLessonId);
                    //newUser.EnglishLayoutLevel = _context.EnglishLayoutLevels.FirstOrDefault(oneLevel =>oneLevel.Id == newUser.EnglishLayoutLevelId);
                    _users.Add(newUser);
                    _context.Entry(newUser).State = EntityState.Added;
                    await SaveChangesAsync();
                    successCode++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
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
