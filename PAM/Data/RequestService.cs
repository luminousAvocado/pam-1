using System;
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
            request.CreatedOn = DateTime.Now;
            _dbContext.Add(request);
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
            return _dbContext.Requests.Include(r => r.RequestType).Include(r => r.RequestedBy).Include(r => r.RequestedFor)
                .Include(r => r.Systems).ThenInclude(rs => rs.System)
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

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
