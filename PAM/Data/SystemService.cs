using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Data
{
    public class SystemService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public SystemService(AppDbContext dbContext, ILogger<SystemService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public RequestedSystem SaveRequestedSystem(RequestedSystem reqSystem)
        {
            _dbContext.Add(reqSystem);
            _dbContext.SaveChanges();
            return reqSystem;
        }
    }
}
