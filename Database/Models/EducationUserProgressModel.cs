using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectKeyboardApplication.Database.Models
{
    public class EducationUserProgressModel : BaseTypingTutorModel
    {
        private DbSet<EducationUsersProgress> _educationUsersProgresses;
        public EducationUserProgressModel(TypingTutorDbContext context):base(context)
        {
            _educationUsersProgresses = _context.EducationUsersProgresses;
        }
        public async Task AddNewEducationUserProgressAsync(EducationUsersProgress educationUser,ILogger logger)
        {
            await Task.Run(() =>
            {
                try
                {
                    _context.Entry(educationUser.User).State = EntityState.Unchanged;
                    _context.Entry(educationUser.EnglishLayoutLesson).State = EntityState.Unchanged;
                    _context.Entry(educationUser.EnglishLayoutLevel).State = EntityState.Unchanged;
                    _educationUsersProgresses.Add(educationUser);
                    logger.LogInformation($"[{nameof(AddNewEducationUserProgressAsync)}] method is success!");
                    SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
            });
           
        }
        public async Task AddRangeNewEducationProgressAsync(IEnumerable<EducationUsersProgress> collection,ILogger logger)
        {
            await Task.Run(() =>
            {
                try
                {
                    foreach (var educationUser in collection)
                    {
                        _context.Entry(educationUser.User).State = EntityState.Unchanged;
                        _context.Entry(educationUser.EnglishLayoutLesson).State = EntityState.Unchanged;
                        _context.Entry(educationUser.EnglishLayoutLevel).State = EntityState.Unchanged;
                        _educationUsersProgresses.Add(educationUser);
                        logger.LogInformation($"[{nameof(AddRangeNewEducationProgressAsync)}] method success!");
                        SaveChangesAsync();
                    }
                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message);
                }

            });
        }
        public async Task<IEnumerable<EducationUsersProgress>> GetUsersEducationProgressAsync(int userId)
        {
            return await _educationUsersProgresses.Where(oneProgress => oneProgress.UserId == userId).ToListAsync();
        }
        public async Task<int> RemoveUsersEducationProgressAsync(int userId)
        {
           return await Task.Run(() =>
            {
                var removeEducationProgressesCollection = _educationUsersProgresses.Where(oneProgress => oneProgress.UserId == userId);
                int removeCount = removeEducationProgressesCollection.Count();
                _educationUsersProgresses.RemoveRange(removeEducationProgressesCollection);
                return removeCount;
            });
            
        }
        public async Task<EducationUsersProgress?> GetNextEducationProgressAsync(EducationUsersProgress currentProgress)
        {
            return await _educationUsersProgresses.Where(oneProg => oneProg.Id > currentProgress.Id)?.OrderBy(oneProg => oneProg.Id).FirstOrDefaultAsync();
        }
    }
}
