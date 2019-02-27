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

        public Models.System GetSystemById(int id){
            return _dbContext.Systems
                .Where(s => s.SystemId.Equals(id)).FirstOrDefault();
        }

        public ICollection<SystemAccess> GetSystemAccess(){
            return _dbContext.SystemAccesses.ToList();
        }

        public ICollection<SystemAccess> GetSystemAccessesByEmployeeId(int empId)
        {
            return _dbContext.SystemAccesses
                .Include(e => e.System)
                .Where(e => e.EmployeeId == empId).ToList();
        }

        public void RemoveSystemAccess(int systemId)
        {
            var systemAccess = _dbContext.SystemAccesses
                .Where(sa => sa.SystemId == systemId).ToList().FirstOrDefault();
            _dbContext.SystemAccesses.Remove(systemAccess);
            _dbContext.SaveChanges();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
