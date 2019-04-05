using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PAM.Models;

namespace PAM.Data
{
    public class FormService
    {
        private readonly AppDbContext _dbContext;

        public FormService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IList<Form> GetForms(List<int> ids)
        {
            return _dbContext.Forms.Where(s => ids.Contains(s.FormId)).ToList();
        }
    }
}
