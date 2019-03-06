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

        public IList<Models.System> GetSystems()
        {
            return _dbContext.Systems.Include(s => s.ProcessingUnit).OrderBy(s => s.Name).ToList();
        }

        public IList<Models.System> GetSystems(List<int> ids)
        {
            return _dbContext.Systems.Where(s => ids.Contains(s.SystemId)).ToList();
        }

        public IList<Models.System> GetSystemsWithoutProcessingUnit()
        {
            return _dbContext.Systems.Where(s => s.ProcessingUnitId == null).OrderBy(s => s.Name).ToList();
        }

        public IList<Models.System> GetSystemsOfProcessingUnit(int processingUnitId)
        {
            return _dbContext.Systems.Where(s => s.ProcessingUnitId == processingUnitId).ToList();
        }

        public Models.System GetSystem(int id)
        {
            return _dbContext.Systems.Where(s => s.SystemId == id).Include(s => s.ProcessingUnit).FirstOrDefault();
        }

        public Models.System AddSystem(Models.System system)
        {
            _dbContext.Systems.Add(system);
            _dbContext.SaveChanges();
            return system;
        }

        public IList<SystemAccess> GetSystemAccessesByEmployeeId(int employeeId)
        {
            return _dbContext.SystemAccesses.Include(s => s.System)
                .Where(s => s.EmployeeId == employeeId)
                .OrderBy(s => s.SystemId).OrderByDescending(s => s.ApprovedOn)
                .ToList();
        }

        public IList<SystemAccess> GetCurrentSystemAccessesByEmployeeId(int employeeId)
        {
            var accesses = GetSystemAccessesByEmployeeId(employeeId);
            var currentAccesses = new Dictionary<int, SystemAccess>();
            foreach (var access in accesses)
                if (!currentAccesses.ContainsKey(access.SystemId))
                    currentAccesses.Add(access.SystemId, access);

            return currentAccesses.Where(a => a.Value.AccessType == SystemAccessType.Add || a.Value.AccessType == SystemAccessType.Update)
                .Select(a => a.Value).ToList();
        }

        public SystemAccess AddSystemAccess(SystemAccess systemAccess)
        {
            _dbContext.SystemAccesses.Add(systemAccess);
            _dbContext.SaveChanges();
            return systemAccess;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
