using KeyboardApplicationRestApiServer.Database.Context;
using KeyboardApplicationRestApiServer.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectKeyboardApplication.Database.Models
{
    public abstract class BaseTypingTutorModel: IBaseDbModel
    {
        protected TypingTutorDbContext _context;

        public BaseTypingTutorModel( TypingTutorDbContext context)
        {
            _context = context;
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync(); // maybe add await
        }
    }
}
