using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Data
{
    public class OrganizationService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public OrganizationService(AppDbContext dbContext, ILogger<OrganizationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public ICollection<Bureau> GetBureaus()
        {
            return _dbContext.Bureaus.ToList();
        }

        public ICollection<Unit> GetUnits()
        {
            return _dbContext.Units.ToList();
        }

        public ICollection<UnitSystem> GetRelatedUnitSystems(int unitId)
        {
            return _dbContext.UnitSystems.Where(x => x.UnitId == unitId).ToList();
        }

        public ICollection<Models.System> GetSystem(int systemId)
        {
            return _dbContext.Systems.Where(x => x.SystemId == systemId).ToList();
        }

        public ICollection<Models.System> GetRelatedSystems(int unitId)
        {
            ICollection<UnitSystem> unitSystemList;
            List<Models.System> systemsList = new List<Models.System>();

            unitSystemList = GetRelatedUnitSystems(unitId);
            
            foreach(UnitSystem unitSystem in unitSystemList)
            {
                // THERE IS A BUG HERE WHERE IT DOESNT GET ALL SYSTEMS
                // TEST WITH 'JUVENILE BUREAU STAFF' unit, it wont get the last system (systemid 64)
                // Implement Sun's method with eagerly

                List<Models.System> temp = GetSystem(unitSystem.SystemId).ToList();
                systemsList.Add(temp.First());
            }
            
            return systemsList;
        }
    }
}
