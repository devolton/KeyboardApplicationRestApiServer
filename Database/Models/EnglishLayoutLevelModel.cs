using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseProjectKeyboardApplication.Database.Models
{
    public class EnglishLayoutLevelModel : BaseTypingTutorModel
    {
        private DbSet<EnglishLayoutLevel> _englishLayoutLevels;
        public EnglishLayoutLevelModel(TypingTutorDbContext context) : base(context)
        {

            _englishLayoutLevels = _context.EnglishLayoutLevels;
            _englishLayoutLevels.Load();
        }
        public async Task<IEnumerable<EnglishLayoutLevel>> GetLevelsAsync()
        {
            return await _englishLayoutLevels.ToListAsync();
        }
        public async Task<int> AddLevelAsync(EnglishLayoutLevel level)
        {
            return await Task.Run(() =>
              {
                  int code = 0;
                  try
                  {
                      _englishLayoutLevels.Add(level);
                      return ++code;
                  }
                  catch (Exception ex)
                  {
                      return code;
                  }
                  
                
              });
        }
        public async Task<int> UpdateLevel(int levelId,EnglishLayoutLevel level)
        {
            var code = 0;
            var levelForUpdate = await _englishLayoutLevels.FirstOrDefaultAsync(oneLevel => oneLevel.Id == levelId);
            if(levelForUpdate is not null)
            {
                levelForUpdate.Title = level.Title;
                levelForUpdate.Ordinal = level.Ordinal;
                levelForUpdate.Users = level.Users;
                levelForUpdate.Lessons = level.Lessons;
                code++;
            }
            return code;     
        }

        public async Task<EnglishLayoutLevel?> GetLevelByIdAsync(int levelId)
        {
            return await _englishLayoutLevels.FirstOrDefaultAsync(oneLevel => oneLevel.Id == levelId);
        }
    }
}
