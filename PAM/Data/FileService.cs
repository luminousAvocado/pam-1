using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PAM.Models;

namespace PAM.Data
{
    public class FileService 
    {
        private readonly AppDbContext _dbContext;

        public FileService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddFile(File file)
        {
            _dbContext.Files.Add(file);
            _dbContext.SaveChanges();
        }

        public File GetFileByName(string name)
        {
            return _dbContext.Files.Where(f => f.Name == name).FirstOrDefault();
        }

        public File GetFileById(int id)
        {
            return _dbContext.Files.Where(f => f.FileId == id).FirstOrDefault();
        }

        public ICollection<File> GetAllFiles()
        {
            return _dbContext.Files.ToList();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
