using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PAM.Models;

namespace PAM.Data
{
    public class RequestService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;

        public RequestService(AppDbContext dbContext, ILogger<RequestService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Request GetRequest(int id){
            return _dbContext.Requests
                .Include(r => r.RequestType).Where(r => r.RequestId.Equals(id)).FirstOrDefault();
        }

        public ICollection<Request> GetRequests(string username)
        {
            return _dbContext.Requests
                .Include(r => r.RequestedBy).Include(r => r.RequestedFor).Include(r => r.RequestType)
                .Where(r => r.RequestedBy.Username == username || r.RequestedFor.Username == username)
                .ToList();
        }

        public Request SaveRequest(Request request)
        {
            if (request.RequestId == 0) _dbContext.Add(request);
            _dbContext.SaveChanges();
            return request;
        }

        public void UpdateRequest (Request request){
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public void RemoveRequest(Request request)
        {
            _dbContext.Remove(request);
            _dbContext.SaveChanges();
        }
    }
}
