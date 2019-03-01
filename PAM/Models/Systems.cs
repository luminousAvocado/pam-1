using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAM.Models
{
    [Table("Systems")]
    public class System
    {
        public int SystemId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Owner { get; set; }
        public bool Retired { get; set; }
    }

    [Table("UnitSystems")]
    public class UnitSystem
    {
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        public int SystemId { get; set; }
        public System System { get; set; }
    }

    public enum SystemAccessType { Add, Remove, Update };

    [Table("RequestedSystems")]
    public class RequestedSystem
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int SystemId { get; set; }
        public System System { get; set; }

        public bool InPortfolio { get; set; } = true;
        public SystemAccessType AccessType { get; set; } = SystemAccessType.Add;

        public RequestedSystem() { }

        public RequestedSystem(int requestId, int systemId)
        {
            RequestId = requestId;
            SystemId = systemId;
        }

        public RequestedSystem(int requestId, int systemId, bool inPortfolio, SystemAccessType accessType)
        {
            RequestId = requestId;
            SystemId = systemId;
            InPortfolio = inPortfolio;
            AccessType = accessType;
        }
    }

    // After a request is approved, each RequestedSystem becomes a SystemAccess
    // associated with an Employee instead of a Requester. 
    [Table("SystemAccesses")]
    public class SystemAccess
    {
        public int SystemAccessId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int SystemId { get; set; }
        public System System { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public DateTime ApprovedOn { get; set; }

        public bool InPortfolio { get; set; }
        public SystemAccessType AccessType { get; set; }

        public int? ProcessedById { get; set; }
        [ForeignKey(nameof(ProcessedById))]
        public Employee ProcessedBy { get; set; }
        public DateTime? ProcessedOn { get; set; }

        public int? ConfirmedById { get; set; }
        [ForeignKey(nameof(ConfirmedById))]
        public Employee ConfirmedBy { get; set; }
        public DateTime? ConfirmedOn { get; set; }

        [NotMapped]
        public bool IsProcessed => ProcessedById != null && ProcessedOn != null;

        [NotMapped]
        public bool IsConfirmed => ConfirmedById != null && ConfirmedOn != null;

        public SystemAccess() { }

        public SystemAccess(Request request, RequestedSystem requestedSystem)
        {
            EmployeeId = request.RequestedFor.EmployeeId;
            SystemId = requestedSystem.SystemId;
            RequestId = request.RequestId;
            ApprovedOn = (DateTime)request.CompletedOn;
            InPortfolio = requestedSystem.InPortfolio;
            AccessType = requestedSystem.AccessType;
        }
    }
}
