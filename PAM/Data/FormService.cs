using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public void AddForm(Form form)
        {
            _dbContext.Forms.Add(form);
            _dbContext.SaveChanges();
        }

        public Form GetFileByName(string name)
        {
            return _dbContext.Forms.Where(f => f.Name == name).Include(f => f.File).FirstOrDefault();
        }

        public Form GetFormById(int id)
        {
            return _dbContext.Forms.Where(f => f.FormId == id).Include(f => f.File).FirstOrDefault();
        }

        public ICollection<Form> GetAllForms()
        {
            return _dbContext.Forms.ToList();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
