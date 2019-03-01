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

        public ICollection<Models.System> GetSystems()
        {
            return _dbContext.Systems.Include(s => s.ProcessingUnit).OrderBy(s => s.Name).ToList();
        }

        public ICollection<Models.System> GetSystemsWithoutProcessingUnit()
        {
            return _dbContext.Systems.Where(s => s.ProcessingUnitId == null).OrderBy(s => s.Name).ToList();
        }

        public ICollection<Models.System> GetSystemsOfProcessingUnit(int processingUnitId)
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

        public IList<Models.System> GetSystems(List<int> ids)
        {
            return _dbContext.Systems.Where(s => ids.Contains(s.SystemId)).ToList();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
