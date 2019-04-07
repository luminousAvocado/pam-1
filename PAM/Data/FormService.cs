using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Data
{
    public class FormService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<FormService> _logger;

        public FormService(AppDbContext dbContext, ILogger<FormService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public List<Form> GetForms()
        {
            return _dbContext.Forms.OrderBy(f => f.DisplayOrder).ThenBy(f => f.FormId).ToList();
        }

        public Form GetForm(int id)
        {
            return _dbContext.Forms.Where(f => f.FormId == id)
                .Include(f => f.Systems).ThenInclude(s => s.System)
                .FirstOrDefault();
        }

        public Form AddForm(Form form)
        {
            _dbContext.Forms.Add(form);
            _dbContext.SaveChanges();
            return form;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
