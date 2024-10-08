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
                    logger.LogInformation($"{DateTime.Now} - [{nameof(AddNewEducationUserProgressAsync)}] method is success!");
                    SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError($"{DateTime.Now} - [{nameof(AddNewEducationUserProgressAsync)}] method error! Message: {ex.Message}");
                }
            });

        }
        public async Task AddRangeNewEducationProgressAsync(IEnumerable<EducationUsersProgress> collection, ILogger logger)
        {

            try
            {
                foreach (var educationUserProgress in collection)
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(oneUser=> oneUser.Id ==educationUserProgress.UserId);
                    var existingLesson = await _context.EnglishLayoutLessons.FirstOrDefaultAsync(oneLesson=> oneLesson.Id == educationUserProgress.EnglishLayoutLessonId);
                    var existingLevel = await _context.EnglishLayoutLevels.FirstOrDefaultAsync(oneLevel=> oneLevel.Id == educationUserProgress.EnglishLayoutLevelId);
                    var userLesson = await _context.EnglishLayoutLessons.FirstOrDefaultAsync(oneLesson=>oneLesson.Id ==(educationUserProgress.EnglishLayoutLesson.Id + 1)) ?? existingLesson;
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

                
                    _educationUsersProgresses.Add(educationUserProgress);
                    _context.Entry(educationUserProgress).State = EntityState.Added;
                }

                await _context.SaveChangesAsync();
                logger.LogInformation($"{DateTime.Now} - [{nameof(AddRangeNewEducationProgressAsync)}] method success! {nameof(collection)} elements count: {collection.Count()}");

            }
            catch (Exception ex)
            {
                logger.LogError($"{DateTime.Now} - [{nameof(AddRangeNewEducationProgressAsync)}] method error! Message: {ex.Message}");

            }
        }
        public async Task<IEnumerable<EducationUsersProgress>> GetUsersEducationProgressAsync(int userId)
        {
            return await _educationUsersProgresses.Where(oneProgress => oneProgress.UserId == userId).ToListAsync();
        }
        public async Task<int> RemoveUsersEducationProgressAsync(int userId, ILogger logger)
        {
            return await Task.Run(() =>
             {
                 try
                 {
                     var removeEducationProgressesCollection = _educationUsersProgresses.Where(oneProgress => oneProgress.UserId == userId);
                     int removeCount = removeEducationProgressesCollection.Count();
                     _educationUsersProgresses.RemoveRange(removeEducationProgressesCollection);
                     return removeCount;
                 }
                 catch (Exception ex)
                 {
                     logger.LogError($"{DateTime.Now} - [{nameof(RemoveUsersEducationProgressAsync)}] method error: {ex.Message}");
                 }
                 return 0;
             });

        }
        public async Task UpdateRangeEducationProgressAsync(IEnumerable<EducationUsersProgress> updatedEducationUserProgressCollection, ILogger logger)
        {
            await Task.Run(async () =>
            {
                try
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
                    logger.LogInformation($"{DateTime.Now} - [{nameof(UpdateEducationProgressAsync)}] method is success!");
                }
                catch (Exception ex)
                {
                    logger.LogError($"{DateTime.Now} - [{nameof(UpdateEducationProgressAsync)}] method error! Message: {ex.Message}");
                }
            });
        }
        public async Task UpdateEducationProgressAsync(EducationUsersProgress updatedEducationUserProgress, ILogger logger)
        {
            await Task.Run(async () =>
            {
                try
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
                    logger.LogInformation($"{DateTime.Now} - [{nameof(UpdateEducationProgressAsync)}] method success!");
                }
                catch (Exception ex)
                {
                    logger.LogError($"{DateTime.Now} - [{nameof(UpdateEducationProgressAsync)}] method error! Message: {ex.Message}");
                }
            });
        }
        public async Task<EducationUsersProgress?> GetNextEducationProgressAsync(EducationUsersProgress currentProgress)
        {
            return await _educationUsersProgresses.Where(oneProg => oneProg.Id > currentProgress.Id)?.OrderBy(oneProg => oneProg.Id)?.FirstOrDefaultAsync();
        }
    }
}
