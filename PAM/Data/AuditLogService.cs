using System;
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

        public async Task Append(int employeeId, LogActionType actionType, LogResourceType resourceType, int resourceId, string message,
            string oldValue = null, string newValue = null)
        {
            var logEntry = new AuditLogEntry(employeeId, actionType, resourceType, resourceId, message, oldValue, newValue);
            _dbContext.AuditLog.Add(logEntry);
            await _dbContext.SaveChangesAsync();
        }

        public IList<AuditLogEntry> GetRecentEntries(int days)
        {
            return _dbContext.AuditLog.Where(e => e.Timestamp > DateTime.Now.AddDays(-days))
                .OrderByDescending(e => e.Timestamp)
                .ToList();
        }
    }
}
