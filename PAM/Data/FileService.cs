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

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
