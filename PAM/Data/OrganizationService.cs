using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public ICollection<UnitSystem> GetRelatedSystems(int unitId)
        {
            // Returns UnitSystem JOIN System. Systems related to the specific unitId
            var unitAndRelatedSystems = _dbContext.UnitSystems.Include(unitSystem => unitSystem.System).Where(x => x.UnitId == unitId).ToList();
            
            return unitAndRelatedSystems;
        }
    }
}
