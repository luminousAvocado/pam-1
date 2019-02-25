using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PAM.Models;

namespace PAM.Data
{
    public class SystemService
    {
        private readonly AppDbContext _dbContext;

        public SystemService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SystemAccess AddSystemAccess(SystemAccess systemAccess)
        {
            _dbContext.SystemAccesses.Add(systemAccess);
            _dbContext.SaveChanges();
            return systemAccess;
        }

        public ICollection<Models.System> GetAllSystems(){
            return _dbContext.Systems.ToList();
        } 

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
