﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PAM.Models
{
    public enum RequestStatus { Draft, UnderReview, Approved, Denied, Processed, Confirmed };

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

        public int? DisplayOrder { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public ICollection<RequiredSignature> RequiredSignatures { get; set; }
    }

    [Table("RequiredSignatures")]
    public class RequiredSignature
    {
        public int RequiredSignatureId { get; set; }

        public int RequestTypeId { get; set; }

        [Required]
        public string Title { get; set; }

        public int Order { get; set; }
    }

    [Table("Requests")]
    public class Request
    {
        public int RequestId { get; set; }

        public int RequestTypeId { get; set; }
        public RequestType RequestType { get; set; }

        public int RequestedById { get; set; }
        [ForeignKey(nameof(RequestedById))]
        public Requester RequestedBy { get; set; }

        public int RequestedForId { get; set; }
        [ForeignKey(nameof(RequestedForId))]
        public Requester RequestedFor { get; set; }

        public RequestStatus RequestStatus { get; set; } = RequestStatus.Draft;

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? SubmittedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

        public bool IsContractor { get; set; } = false;
        public bool IsHighProfileAccess { get; set; } = false;
        public bool IsGlobalAccess { get; set; } = false;

        public List<RequestedSystem> Systems { get; set; } = new List<RequestedSystem>();

        public List<CompletedForm> Forms { get; set; } = new List<CompletedForm>();

        public ICollection<Review> Reviews { get; set; }

        public CaseloadType? CaseloadType { get; set; }
        public CaseloadFunction? CaseloadFunction { get; set; }
        public string CaseloadNumber { get; set; }
        public string OldCaseloadNumber { get; set; }

        public int? TransferredFromUnitId { get; set; }
        public Unit TransferredFromUnit { get; set; }

        public DepartureReason? DepartureReason { get; set; }

        public string IpAddress { get; set; }

        public string Notes { get; set; }

        public bool Deleted { get; set; } = false;

        [NotMapped]
        public string DisplayId => SubmittedOn?.Year + "-" + RequestId.ToString("D6");

        [NotMapped]
        public bool IsSelfRequest => RequestedById == RequestedForId;

        [NotMapped]
        public List<Review> OrderedReviews => Reviews != null ? Reviews.OrderBy(r => r.ReviewOrder).ToList() : null;
    }

    [Table("Reviews")]
    public class Review
    {
        public int ReviewId { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int? ReviewerId { get; set; }
        public Employee Reviewer { get; set; }

        public int ReviewOrder { get; set; }

        [Required]
        public string ReviewerTitle { get; set; }

        public bool? Approved { get; private set; }
        public string Comments { get; private set; }
        public DateTime? Timestamp { get; private set; }

        [NotMapped]
        public string ReviewerName { get; set; }

        [NotMapped]
        public bool Completed => Approved != null;

        public void Approve(string comments)
        {
            Approved = true;
            Comments = comments;
            Timestamp = DateTime.Now;
        }

        public void Deny(string comments)
        {
            Approved = false;
            Comments = comments;
            Timestamp = DateTime.Now;
        }
    }
}
