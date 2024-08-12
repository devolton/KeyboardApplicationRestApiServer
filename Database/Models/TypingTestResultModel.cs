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
    public class TypingTestResultModel : BaseTypingTutorModel
    {
        private DbSet<TypingTestResult> _typingTestResults;
        public TypingTestResultModel(TypingTutorDbContext context) : base(context)
        {
            _typingTestResults = _context.TypingTestResults;
        }
        public async Task<IEnumerable<TypingTestResult>> GetTypingTestResultsByUserIdAsync(int userId)
        {
            return await _typingTestResults.Where(oneTestResult => oneTestResult.UserId == userId).ToListAsync();
        }
        public async Task<int> RemoveUsersTestAsync(int userId)
        {
            return await Task.Run(() =>
            {

                var removeTestResultsCollection = _typingTestResults.Where(oneResult => oneResult.UserId.Equals(userId));
                int removeCount = removeTestResultsCollection.Count();
                _typingTestResults.RemoveRange(removeTestResultsCollection);
                return removeCount;
            });

        }
        public async Task<int> AddNewTypingTestResultAsync(TypingTestResult typingTestResult)
        {
            return await Task.Run(async () =>
            {
                int successCode = 0;
                try
                {
                    _context.Entry(typingTestResult.User).State = EntityState.Unchanged;
                    _context.Entry(typingTestResult).State = EntityState.Added;
                    _typingTestResults.Add(typingTestResult);
                    await SaveChangesAsync();
                    return ++successCode;
                }
                catch
                {
                    return successCode;
                }
            });
        }
        public async Task<int> AddRangeTypingTestResultsAsync(IEnumerable<TypingTestResult> typingTestResultCollection ,ILogger logger)
        {
            int successCode = 0;
            try
            {
                if (typingTestResultCollection.Count() > 0 && typingTestResultCollection is not null)
                {
                    logger.LogInformation(typingTestResultCollection.First().ToString());
                }
                foreach (var  typingTestResult in typingTestResultCollection)
                {
                    typingTestResult.User = _context.Users.FirstOrDefault(oneUser => oneUser.Id == typingTestResult.User.Id);
                    _context.Entry(typingTestResult.User).State = EntityState.Unchanged;
                    _context.Entry(typingTestResult).State = EntityState.Added;
                    _typingTestResults.Add(typingTestResult);
                }
                
                await SaveChangesAsync();
                return ++successCode;
                
            }
            catch(Exception ex)
            {
                logger.LogError(ex.ToString());
                return successCode;
            }


        }

        public async Task<TypingTestResult?> GetBestUserTestResultAsync(int userId)
        {
            return await _typingTestResults.Where(oneResult => oneResult.UserId.Equals(userId))?.OrderByDescending(oneResult => oneResult.Speed).FirstOrDefaultAsync();
        }

    }
}
