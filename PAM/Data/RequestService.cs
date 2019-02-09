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
                .Include(r => r.RequestType)
                .Include(r => r.RequestedFor)
                .Where(r => r.RequestId.Equals(id)).FirstOrDefault();
        }

        public ICollection<Request> GetRequestsByUsername(string username)
        {
            return _dbContext.Requests
                .Include(r => r.RequestedBy).Include(r => r.RequestedFor).Include(r => r.RequestType)
                .Where(r => r.RequestedBy.Username == username || r.RequestedFor.Username == username)
                .ToList();
        }

        public ICollection<Request> GetRequests(){
            return _dbContext.Requests
                .Include(r => r.RequestedBy).Include(r => r.RequestedFor).Include(r => r.RequestType)
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

        public ICollection<Request> GetRequestedSystemsByRequestId(int requestId)
        {
            return _dbContext.Requests
                .Include(x => x.Systems)
                .Where(x => x.RequestId == requestId)
                .ToList();
        }

        public Review SaveReview(Review review)
        {
            _dbContext.Add(review);
            _dbContext.SaveChanges();
            return review;
        }

        public void UpdateReview(Review review)
        {
            _dbContext.Update(review);
            _dbContext.SaveChanges();
        }

        public ICollection<Review> GetRequestsForReview(int supervisorId)
        {
            // Currently this will not include RequestType, so we cant show RequestType name
            var relatedRequests = _dbContext.Reviews
                .Include(x => x.Request)
                .Where(x => x.ReviewerId == supervisorId)
                .ToList();

            return relatedRequests;
        }
    }
}
