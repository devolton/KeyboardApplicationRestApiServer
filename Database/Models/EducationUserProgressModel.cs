﻿using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectKeyboardApplication.Database.Models
{
    public class EducationUserProgressModel : BaseTypingTutorModel
    {
        private DbSet<EducationUsersProgress> _educationUsersProgresses;
        public EducationUserProgressModel(TypingTutorDbContext context) : base(context)
        {
            _educationUsersProgresses = _context.EducationUsersProgresses;
        }
        public async Task AddNewEducationUserProgressAsync(EducationUsersProgress educationUserProgress, ILogger logger)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(educationUserProgress.User.Id);
                    var existingLesson = await _context.EnglishLayoutLessons.FindAsync(educationUserProgress.EnglishLayoutLesson.Id);
                    var existingLevel = await _context.EnglishLayoutLevels.FindAsync(educationUserProgress.EnglishLayoutLevel.Id);

                    if (existingUser != null)
                    {
                        existingUser.EnglishLayoutLevelId = educationUserProgress.EnglishLayoutLevelId;
                        existingUser.EnglishLayoutLessonId = educationUserProgress.EnglishLayoutLessonId;
                        educationUserProgress.User = existingUser;
                        _context.Entry(existingUser).State = EntityState.Modified;
                    }

                    if (existingLesson != null)
                    {
                        educationUserProgress.EnglishLayoutLesson = existingLesson;
                        _context.Entry(existingLesson).State = EntityState.Unchanged;
                    }

                    if (existingLevel != null)
                    {
                        educationUserProgress.EnglishLayoutLevel = existingLevel;
                        _context.Entry(existingLevel).State = EntityState.Unchanged;
                    }
                    _context.Entry(educationUserProgress).State = EntityState.Added;
                    _educationUsersProgresses.Add(educationUserProgress);
                    logger.LogInformation($"[{nameof(AddNewEducationUserProgressAsync)}] method is success!");
                    SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
            });

        }
        public async Task AddRangeNewEducationProgressAsync(IEnumerable<EducationUsersProgress> collection, ILogger logger)
        {
            await Task.Run(async () =>
            {
                try
                {
                    foreach (var educationUserProgress in collection)
                    {
                        var existingUser = await _context.Users.FindAsync(educationUserProgress.User.Id);
                        var existingLesson = await _context.EnglishLayoutLessons.FindAsync(educationUserProgress.EnglishLayoutLesson.Id);
                        var existingLevel = await _context.EnglishLayoutLevels.FindAsync(educationUserProgress.EnglishLayoutLevel.Id);
                        var userLesson = await _context.EnglishLayoutLessons.FindAsync(educationUserProgress.EnglishLayoutLesson.Id + 1) ?? existingLesson;
                        var userLevel = userLesson.EnglishLayoutLevel;


                        if (existingUser != null)
                        {
                            existingUser.EnglishLayoutLevel = userLevel;
                            existingUser.EnglishLayoutLevelId = userLevel.Id;
                            existingUser.EnglishLayoutLesson = userLesson;
                            existingUser.EnglishLayoutLessonId = userLesson.Id;

                            educationUserProgress.User = existingUser;
                            _context.Entry(existingUser).State = EntityState.Modified;
                        }

                        if (existingLesson != null)
                        {
                            educationUserProgress.EnglishLayoutLesson = existingLesson;
                            _context.Entry(existingLesson).State = EntityState.Unchanged;
                        }

                        if (existingLevel != null)
                        {
                            educationUserProgress.EnglishLayoutLevel = existingLevel;
                            _context.Entry(existingLevel).State = EntityState.Unchanged;
                        }

                        _context.Entry(educationUserProgress).State = EntityState.Added;
                        _educationUsersProgresses.Add(educationUserProgress);
                    }

                    logger.LogInformation($"[{nameof(AddRangeNewEducationProgressAsync)}] method success! {nameof(_educationUsersProgresses)} elements count: {_educationUsersProgresses.Count()}");
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateException ex)
                {
                    var innerException = ex.InnerException?.Message;
                    logger.LogError(innerException);
                    string str = string.Empty;
                    foreach (var item in ex.Entries)
                    {
                        str += item.ToString() + " ";
                    }
                    logger.LogError(str);
                   
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
        public async Task UpdateRangeEducationProgressAsync(IEnumerable<EducationUsersProgress> updatedEducationUserProgressCollection)
        {
            await Task.Run(async () =>
            {
                foreach (var updatedEducationUserProgress in updatedEducationUserProgressCollection)
                {
                    EducationUsersProgress? educationUserProgress = _context.EducationUsersProgresses?.FirstOrDefault(oneEducProg => oneEducProg.Id == updatedEducationUserProgress.Id);
                    if (educationUserProgress is not null)
                    {
                        educationUserProgress.IsWithoutErrorsCompleted = updatedEducationUserProgress.IsWithoutErrorsCompleted;
                        educationUserProgress.IsLessThanTwoErrorsCompleted = updatedEducationUserProgress.IsLessThanTwoErrorsCompleted;
                        educationUserProgress.IsSpeedCompleted = updatedEducationUserProgress.IsSpeedCompleted;
                        _context.Entry(educationUserProgress).State = EntityState.Modified;
                       
                    }
                }
                await SaveChangesAsync();
            });
        }
        public async Task UpdateEducationProgressAsync(EducationUsersProgress updatedEducationUserProgress)
        {
            await Task.Run(async () =>
            {

                    EducationUsersProgress? educationUserProgress = _context.EducationUsersProgresses?.FirstOrDefault(oneEducProg => oneEducProg.Id == updatedEducationUserProgress.Id);
                    if (educationUserProgress is not null)
                    {
                        educationUserProgress.IsWithoutErrorsCompleted = updatedEducationUserProgress.IsWithoutErrorsCompleted;
                        educationUserProgress.IsLessThanTwoErrorsCompleted = updatedEducationUserProgress.IsLessThanTwoErrorsCompleted;
                        educationUserProgress.IsSpeedCompleted = updatedEducationUserProgress.IsSpeedCompleted;
                        _context.Entry(educationUserProgress).State = EntityState.Modified;

                    }
                
                await SaveChangesAsync();
            });
        }
        public async Task<EducationUsersProgress?> GetNextEducationProgressAsync(EducationUsersProgress currentProgress)
        {
            return await _educationUsersProgresses.Where(oneProg => oneProg.Id > currentProgress.Id)?.OrderBy(oneProg => oneProg.Id)?.FirstOrDefaultAsync();
        }
    }
}
