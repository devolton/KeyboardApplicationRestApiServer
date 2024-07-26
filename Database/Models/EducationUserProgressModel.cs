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
        public async Task AddNewEducationUserProgressAsync(EducationUsersProgress educationUser)
        {
            await Task.Run(() =>
            {
                _educationUsersProgresses.Add(educationUser);
            });
           
        }
        public async Task AddRangeNewEducationProgressAsync(IEnumerable<EducationUsersProgress> collection)
        {
            await Task.Run(() =>
            {
                _educationUsersProgresses.AddRange(collection);
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
