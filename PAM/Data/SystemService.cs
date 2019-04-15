﻿using System.Collections.Generic;
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
            return _dbContext.Systems.Include(s => s.SupportUnit).Include(s => s.Forms).OrderBy(s => s.Name).ToList();
        }

        public IList<Models.System> GetSystems(List<int> ids)
        {
            return _dbContext.Systems.Where(s => ids.Contains(s.SystemId)).ToList();
        }

        public IList<Models.System> GetSystemsWithoutSupportUnit()
        {
            return _dbContext.Systems.Where(s => s.SupportUnitId == null).OrderBy(s => s.Name).ToList();
        }

        public IList<Models.System> GetSystemsOfSupportUnit(int supportUnitId)
        {
            return _dbContext.Systems.Where(s => s.SupportUnitId == supportUnitId).ToList();
        }

        public Models.System GetSystem(int id)
        {
            return _dbContext.Systems.Where(s => s.SystemId == id)
                .Include(s => s.SupportUnit).Include(s => s.Forms).ThenInclude(f => f.Form).FirstOrDefault();
        }

        public Models.System AddSystem(Models.System system)
        {
            _dbContext.Systems.Add(system);
            _dbContext.SaveChanges();
            return system;
        }

        public IList<SystemAccess> GetSystemAccesses(List<int> ids)
        {
            return _dbContext.SystemAccesses.Where(s => ids.Contains(s.SystemAccessId))
                .Include(s => s.System).Include(s => s.ProcessedBy).Include(s => s.ConfirmedBy)
                .Include(s => s.Request).ThenInclude(r => r.RequestedBy)
                .Include(s => s.Request).ThenInclude(r => r.RequestedFor)
                .ToList();
        }

        public IList<SystemAccess> GetSystemAccessesByEmployeeId(int employeeId)
        {
            return _dbContext.SystemAccesses.Include(s => s.System).Include(s => s.ProcessedBy).Include(s => s.ConfirmedBy)
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

            return currentAccesses.Where(a => a.Value.AccessType == SystemAccessType.Add || a.Value.AccessType == SystemAccessType.UpdateInfo)
                .Select(a => a.Value).ToList();
        }

        public IList<SystemAccess> GetCurrentSystemAccessesBySupportUnitId(int supportUnitId)
        {
            return _dbContext.SystemAccesses.Include(s => s.System)
                .Include(s => s.Request).ThenInclude(r => r.RequestType)
                .Include(s => s.Request).ThenInclude(r => r.RequestedFor)
                .Include(s => s.ProcessedBy).Include(s => s.ConfirmedBy)
                .Where(s => s.System.SupportUnitId == supportUnitId && (s.ProcessedOn == null || s.ConfirmedOn == null))
                .OrderBy(s => s.RequestId)
                .ToList();
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
