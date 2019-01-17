using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PAM.Extensions;

namespace PAM.Services
{
    public class SessionHelper
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public T GetSessionObj<T> (string name)
        {
            var sessObj = _session.GetObject<T>(name);
            return sessObj;
        }
    }
}
