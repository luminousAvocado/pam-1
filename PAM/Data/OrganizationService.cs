using System.Collections.Generic;
using System.Linq;
using PAM.Models;

namespace PAM.Data
{
    public class OrganizationService
    {
        private readonly AppDbContext _dbContext;

        public OrganizationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ICollection<Bureau> GetBureaus()
        {
            return _dbContext.Bureaus.ToList();
        }
    }
}
