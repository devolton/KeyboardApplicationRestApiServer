using KeyboardApplicationRestApiServer.Database.Context;
using System.Data.Entity;

namespace KeyboardApplicationRestApiServer.Shared.Interfaces
{
    public interface IBaseDbModel
    {
        
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
