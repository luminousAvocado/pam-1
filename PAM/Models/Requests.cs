using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PAM.Models
{
    public enum RequestStatus { Draft, PendingReview, Approved, Denied };

    public enum CaseloadType { Adult, Juvenile, SchoolBased };

    public enum CaseloadFunction { Investigation, Supervision };

    public enum DepartureReason { Terminated, Retired, Resigned };

    [Table("RequestTypes")]
    public class RequestType
    {
        public int RequestTypeId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string DisplayCode { get; set; }

        public string Description { get; set; }
    }

    [Table("Requests")]
    public class Request
    {
        public int RequestId { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }

        public int RequestedById { get; set; }
        [ForeignKey(nameof(RequestedById))]
        public Requester RequestedBy { get; set; }

        public int RequestedForId { get; set; }
        [ForeignKey(nameof(RequestedForId))]
        public Requester RequestedFor { get; set; }

        public RequestStatus RequestStatus { get; set; } = RequestStatus.Draft;

        public DateTime SubmittedOn { get; set; }

        public bool IsContractor { get; set; } = false;
        public bool IsHighProfileAccess { get; set; } = false;
        public bool IsGlobalAccess { get; set; } = false;

        public ICollection<RequestedSystem> Systems { get; private set; }

        public ICollection<Review> Reviews { get; private set; }

        public CaseloadType CaseloadType { get; set; }
        public CaseloadFunction CaseloadFunction { get; set; }
        public string CaseloadNumber { get; set; }
        public string OldCaseloadNumber { get; set; }

        public int? TransferredFromUnitId { get; set; }
        public Unit TransferredFromUnit { get; set; }

        public DepartureReason DepartureReason { get; set; }

        public string IpAddress { get; set; }

        public string Notes { get; set; }

        [NotMapped]
        public bool IsSelfRequest => RequestedById == RequestedForId;
        /*
        [NotMapped]
        public List<Review> OrderedReviews => Reviews.OrderBy(r => r.ReviewOrder).ToList();
        */
    }

    [Table("Reviews")]
    public class Review
    {
        public int ReviewId { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int ReviewerId { get; set; }
        public Employee Reviewer { get; set; }

        public int ReviewOrder { get; set; }

        [Required]
        public string ReviewerTitle { get; set; }

        public bool? Approved { get; private set; }
        public string Comments { get; private set; }
        public DateTime Timestamp { get; private set; }

        [NotMapped]
        public bool Completed => Approved != null;

        public void Approve()
        {
            Approved = true;
            Timestamp = new DateTime();
        }

        public void Deny(string comments)
        {
            Approved = false;
            Comments = comments;
            Timestamp = new DateTime();
        }
    }
}
