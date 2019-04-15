using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Data
{
    public class AuditLogService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(AppDbContext dbContext, ILogger<AuditLogService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public AuditLogEntry GetAuditLogEntry(int id)
        {
            return _dbContext.AuditLog.Include(e => e.Employee)
                .Where(e => e.AuditLogEntryId == id).FirstOrDefault();
        }

        public async Task Append(int employeeId, LogActionType actionType, LogResourceType resourceType, int resourceId, string message,
            string oldValue = null, string newValue = null)
        {
            var logEntry = new AuditLogEntry(employeeId, actionType, resourceType, resourceId, message, oldValue, newValue);
            _dbContext.AuditLog.Add(logEntry);
            await _dbContext.SaveChangesAsync();
        }

        public IList<AuditLogEntry> Search(DateTime startTime, DateTime? endTime = null, string term = null)
        {
            _logger.LogInformation(startTime + " " + endTime + " " + term);
            var query = _dbContext.AuditLog.Where(e => e.Timestamp >= startTime);
            if (endTime != null)
                query = query.Where(e => e.Timestamp <= endTime);
            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(e => EF.Functions.FreeText(e.Message, term));
            return query.OrderByDescending(e => e.Timestamp).ToList();
        }
    }
}
