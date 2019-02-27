using System.Collections.Generic;
using System.Diagnostics;
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

        public void RemoveSystemAccess(int systemId)
        {
            var systemAccess = _dbContext.SystemAccesses
                .Where(sa => sa.SystemId == systemId).ToList().FirstOrDefault();
            _dbContext.SystemAccesses.Remove(systemAccess);
            _dbContext.SaveChanges();
        }

        public ICollection<Models.SystemAccess> GetSystemAccessesByEmployeeId(int empId)
        {
            return _dbContext.SystemAccesses
                .Include(e => e.System)
                .Where(e => e.EmployeeId == empId).ToList();
        }

        public ICollection<Models.System> GetSystems()
        {
            return _dbContext.Systems.OrderBy(s => s.Name).ToList();
        }

        public Models.System GetSystem(int id)
        {
            return _dbContext.Systems.Find(id);
        }

        public Models.System AddSystem(Models.System system)
        {
            _dbContext.Systems.Add(system);
            _dbContext.SaveChanges();
            return system;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
