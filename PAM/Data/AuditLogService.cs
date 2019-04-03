﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PAM.Models;

namespace PAM.Data
{
    public class AuditLogService
    {
        private readonly AppDbContext _dbContext;

        public AuditLogService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICollection<AuditLog> GetAllLogs()
        {
            return _dbContext.AuditLogs.OrderByDescending(x => x.AuditLogId).ToList();
        }

        public ICollection<AuditLog> GetLogsInRange(DateTime start, DateTime end)
        {
            return _dbContext.AuditLogs.OrderByDescending(x => x.AuditLogId)
                .Where(x => x.Timestamp >= start)
                .Where(x => x.Timestamp <= end)
                .ToList();
        }

        public AuditLog CreateAuditLog(Int32 empId, Models.Action action, ResourceType resType, int resId, string oldValue, string newValue)
        {
            AuditLog newLog = new AuditLog
            {
                EmployeeId = empId,
                Action = action,
                ResourceType = resType,
                ResourceId = resId,
                OldValue = oldValue,
                NewValue = newValue
            };

            _dbContext.AuditLogs.Add(newLog);
            _dbContext.SaveChanges();
            return newLog;
        }
    }
}
