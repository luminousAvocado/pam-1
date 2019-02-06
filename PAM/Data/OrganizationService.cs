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
            var unitAndRelatedSystems = _dbContext.UnitSystems.Include(u => u.System)
                            .Include(u => u.Unit)
                            .Where(x => x.UnitId == unitId)
                            .ToList();

            // Block below is for debugging
            var unitSys = _dbContext.UnitSystems.Where(x => x.UnitId == unitId).ToList();
            Debug.WriteLine("*** UnitSystem");
            foreach(var item in unitSys)
            {
                Debug.WriteLine("UnitId: {0}, SystemId: {1}", item.UnitId, item.SystemId);
            }          
            Debug.WriteLine("*** COUNT UnitSystems WITH Systems: {0}", unitAndRelatedSystems.Count);
            foreach(var item in unitAndRelatedSystems)
            {
                Debug.WriteLine("*** UnitId: {0}, *** SystemId/Name: {1}, {2}", item.UnitId, item.System.SystemId, item.System.Name);
            }
            // End debug

            return unitAndRelatedSystems;
        }
    }
}
