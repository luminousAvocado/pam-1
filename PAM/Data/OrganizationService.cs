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

        //public ICollection<System> GetSystem(int systemId)
        //{

        //}
    }
}
