﻿using System.Collections.Generic;
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

        public Location GetLocation(int id)
        {
            return _dbContext.Locations.Find(id);
        }

        public Location AddLocation(Location location)
        {
            _dbContext.Locations.Add(location);
            _dbContext.SaveChanges();
            return location;
        }

        public ICollection<BureauType> GetBureauTypes()
        {
            return _dbContext.BureauTypes.OrderBy(t => t.DisplayCode).ToList();
        }

        public ICollection<Bureau> GetBureaus()
        {
            return _dbContext.Bureaus.OrderBy(b => b.DisplayOrder).ThenBy(b => b.Code).
                AsNoTracking().ToList();
        }

        public Bureau GetBureau(int id)
        {
            return _dbContext.Bureaus.Where(u => u.BureauId == id).Include(u => u.BureauType).FirstOrDefault();
        }

        public Bureau AddBureau(Bureau bureau)
        {
            _dbContext.Bureaus.Add(bureau);
            _dbContext.SaveChanges();
            return bureau;
        }

        public ICollection<Unit> GetUnits()
        {
            return _dbContext.Units.OrderBy(u => u.BureauId).ThenBy(u => u.ParentId).ThenBy(u => u.DisplayOrder).ThenBy(u => u.Name).
                AsNoTracking().ToList();
        }

        public Unit GetUnit(int id)
        {
            return _dbContext.Units.Where(u => u.UnitId == id).Include(u => u.Bureau)
                .Include(u => u.Systems).ThenInclude(us => us.System)
                .FirstOrDefault();
        }

        public ICollection<Models.System> GetAllSystems()
        {
            return _dbContext.Systems.ToList();
        }

        public ICollection<Unit> GetBureauChildren(int bureauId)
        {
            return _dbContext.Units.Where(u => u.BureauId == bureauId).ToList();
        }

        public ICollection<Unit> GetUnitChildren(int parentId)
        {
            return _dbContext.Units.Where(u => u.ParentId == parentId).ToList();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
