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

        public IList<Form> GetForms()
        {
            return _dbContext.Forms.ToList();
        }

        public SystemForm AddSystemForm(SystemForm sysForm)
        {
            _dbContext.SystemForms.Add(sysForm);
            _dbContext.SaveChanges();
            return sysForm;
        }

        public void DeleteSystemForm(int sysId, int formId)
        {
            var sysForm = _dbContext.SystemForms.Where(s => s.SystemId == sysId && s.FormId == formId).First();
            _dbContext.SystemForms.Remove(sysForm);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
