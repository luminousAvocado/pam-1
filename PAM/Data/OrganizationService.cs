using System.Collections.Generic;
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

        public ICollection<Location> GetLocations()
        {
            return _dbContext.Locations.OrderBy(l => l.Name).ToList();
        }

        public Location GetLocation(int i)
        {
            return _dbContext.Locations.Find(i);
        }

        public Location AddLocation(Location location)
        {
            _dbContext.Locations.Add(location);
            _dbContext.SaveChanges();
            return location;
        }

        public ICollection<Bureau> GetBureaus()
        {
            return _dbContext.Bureaus.OrderBy(b => b.DisplayOrder).ThenBy(b => b.Code).
                AsNoTracking().ToList();
        }

        public ICollection<Unit> GetUnits()
        {
            return _dbContext.Units.OrderBy(u => u.BureauId).ThenBy(u => u.ParentId).ThenBy(u => u.DisplayOrder).ThenBy(u => u.Name).
                AsNoTracking().ToList();
        }

        public Unit GetUnit(int id)
        {
            return _dbContext.Units.Where(u => u.UnitId == id)
                .Include(u => u.Systems).ThenInclude(us => us.System)
                .FirstOrDefault();
        }

        public ICollection<UnitSystem> GetRelatedSystems(int unitId)
        {
            // Returns UnitSystem JOIN System. Systems related to the specific unitId
            var unitAndRelatedSystems = _dbContext.UnitSystems.Include(u => u.System)
                            .Include(u => u.Unit)
                            .Where(x => x.UnitId == unitId)
                            .ToList();

            return unitAndRelatedSystems;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
