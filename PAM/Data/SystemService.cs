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

        public ICollection<Models.SystemAccess> GetSystemAccessesByEmployeeId(int empId)
        {
            // *** TODO: Should only include Systems that they have access to, not ones that have been removed
            /*var*/ return /*allSystemAccesses =*/ _dbContext.SystemAccesses
                .Include(e => e.System)
                .Where(e => e.EmployeeId == empId).ToList();

            //allSystemAccesses.
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
