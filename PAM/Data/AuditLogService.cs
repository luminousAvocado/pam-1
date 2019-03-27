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

        public ICollection<AuditLog> GetAllLogs()
        {
            return _dbContext.AuditLogs.ToList();
        }
    }
}
