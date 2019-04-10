using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PAM.Models;

namespace PAM.Data
{
    public class RequestService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public RequestService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public RequestType GetRequestType(int id)
        {
            return _dbContext.RequestTypes.Include(t => t.RequiredSignatures)
                .Where(t => t.RequestTypeId == id).FirstOrDefault();
        }

        public IList<RequestType> GetRequestTypes()
        {
            return _dbContext.RequestTypes.Where(t => t.Enabled).OrderBy(t => t.DisplayOrder).ToList();
        }

        public IList<RequestType> GetAllRequestTypes()
        {
            return _dbContext.RequestTypes.OrderBy(t => t.RequestTypeId).ToList();
        }

        public Request CreateRequest(Request request)
        {
            _dbContext.Requests.Add(request);
            _dbContext.SaveChanges();
            return request;
        }

        public Request UpdateRequest(Request update)
        {
            var request = _dbContext.Requests.Find(update.RequestId);
            _mapper.Map(update, request);
            _dbContext.SaveChanges();
            return request;
        }

        public Request GetRequest(int id)
        {
            return _dbContext.Requests.Include(r => r.RequestType).ThenInclude(rt => rt.RequiredSignatures).Include(r => r.RequestedBy)
                .Include(r => r.RequestedFor).ThenInclude(rr => rr.Unit).ThenInclude(u => u.Bureau)
                .Include(r => r.TransferredFromUnit).ThenInclude(u => u.Bureau)
                .Include(r => r.Systems).ThenInclude(rs => rs.System).ThenInclude(s => s.Forms)
                .Include(r => r.Forms).ThenInclude(ff => ff.File)
                .Include(r => r.Forms).ThenInclude(ff => ff.Form)
                .Include(r => r.Reviews).ThenInclude(rr => rr.Reviewer)
                .Where(r => r.RequestId == id).FirstOrDefault();
        }

        public void RemoveRequest(int id)
        {
            var request = _dbContext.Requests.Find(id);
            request.Deleted = true;
            _dbContext.SaveChanges();
        }

        public ICollection<Request> GetRequestsByUsername(string username)
        {
            return _dbContext.Requests
                .Include(r => r.RequestedBy).Include(r => r.RequestedFor).Include(r => r.RequestType)
                .Where(r => r.RequestedBy.Username == username)
                .ToList();
        }

        public ICollection<Request> GetRequests()
        {
            return _dbContext.Requests
                .Include(r => r.RequestedBy).Include(r => r.RequestedFor).Include(r => r.RequestType)
                .ToList();
        }

        public ICollection<Review> GetReviewsByReviewerId(int reviewerId)
        {
            return _dbContext.Reviews.Include(r => r.Reviewer)
                .Include(r => r.Request).ThenInclude(rr => rr.RequestedFor)
                .Include(r => r.Request).ThenInclude(rr => rr.RequestType)
                .Where(r => r.ReviewerId == reviewerId && r.Request.RequestStatus != RequestStatus.Draft).ToList();
        }

        public Review GetReview(int id)
        {
            return _dbContext.Reviews.Find(id);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
