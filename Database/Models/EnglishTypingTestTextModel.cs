﻿using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Database.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectKeyboardApplication.Database.Models
{
    public class EnglishTypingTestTextModel:BaseTypingTutorModel
    {
        private DbSet<EnglishTypingTestText> _englishTypingTestTexts;
        public EnglishTypingTestTextModel(TypingTutorDbContext context):base(context)
        {
            _englishTypingTestTexts = _context.EnglishTypingTestTexts;
        }
        public async Task<int> AddNewTextAsync(EnglishTypingTestText text, ILogger logger)
        {
            return await Task.Run(() =>
            {
                var successCode = 0;
                try
                {
                    _englishTypingTestTexts.Add(text);
                    SaveChangesAsync();
                    return ++successCode;
                }
                catch(Exception ex)
                {
                    logger.LogError($"{DateTime.Now} - [{nameof(AddNewTextAsync)}] method error: {ex.Message}");
                }

                return successCode;
            });
        }
        
        public async  Task<IEnumerable<EnglishTypingTestText>> GetAllTextsAsync() {
            return await _englishTypingTestTexts.ToListAsync();
        }
        public async Task<EnglishTypingTestText?> GetTextByIdAsync(int id)
        {
            return await _englishTypingTestTexts.FirstOrDefaultAsync(oneText => oneText.Id == id);
        }
        public async Task<int> RemoveTextByIdAsync(int id, ILogger logger)
        {
            int successCode = 0;
            try
            {
                var removerText = await _englishTypingTestTexts.FirstOrDefaultAsync(oneText => oneText.Id == id);
                if (removerText is not null)
                {
                    _englishTypingTestTexts.Remove(removerText);
                    successCode++;
                }
            }
            catch(Exception ex)
            {
                logger.LogError($"{DateTime.Now} - [{nameof(RemoveTextByIdAsync)}] method error: {ex.Message}");
            }
            
            return successCode;

        }
    }
}
