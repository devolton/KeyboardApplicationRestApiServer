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
    public class EnglishLayoutLessonModel : BaseTypingTutorModel
    {
        private DbSet<EnglishLayoutLesson> _englishLayoutLessons;

        public EnglishLayoutLessonModel(TypingTutorDbContext context) : base(context)
        {
            _englishLayoutLessons = _context.EnglishLayoutLessons;
        }
        public async Task<IEnumerable<EnglishLayoutLesson>> GetAllLessonsAsync()
        {
            return await _englishLayoutLessons.ToListAsync();
        }
        public async Task<IEnumerable<EnglishLayoutLesson>> GetLessonsByLevelIdAsync(int levelId)
        {
            return await _englishLayoutLessons.Where(oneLesson => oneLesson.EnglishLayoutLevelId == levelId).ToListAsync();
        }
        public async Task<int> UpdateLessonAsync(int lessonId, EnglishLayoutLesson lesson, ILogger logger)
        {
            var code = 0;
            try
            {
                var lessonForUpdate = await _englishLayoutLessons.FirstOrDefaultAsync(oneLesson => oneLesson.Id == lessonId);
                if (lessonForUpdate is not null)
                {
                    lessonForUpdate.Text = lesson.Text;
                    lessonForUpdate.Ordinal = lesson.Ordinal;
                    lessonForUpdate.Users = lesson.Users;
                    lessonForUpdate.EducationUsersProgresses = lesson.EducationUsersProgresses;
                    lessonForUpdate.EnglishLayoutLevel = lesson.EnglishLayoutLevel;

                    code++;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"{DateTime.Now} - [{nameof(UpdateLessonAsync)}] method error: {ex.Message}");
            }
            return code;
        }
        public async Task<EnglishLayoutLesson?> GetNextLessonAsync(EnglishLayoutLesson currentLesson)
        {
            return await _englishLayoutLessons.Where(oneLesson => oneLesson.Id > currentLesson.Id)?.OrderBy(oneLesson => oneLesson.Id).FirstOrDefaultAsync();

        }
    }
}
