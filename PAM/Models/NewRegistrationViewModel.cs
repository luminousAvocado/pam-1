using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.Models
{
    public class NewRegistrationViewModel
    {

        public Requester RequestedBy { get; set; }
        public Requester RequestedFor { get; set; }
        public Request Request { get; set; }

        public NewRegistrationViewModel() { }

        public NewRegistrationViewModel(Requester requestedBy, Requester requestedFor, Request request)
        {
            RequestedBy = requestedBy;
            RequestedFor = requestedFor;
            Request = request;
        }
    }
}
