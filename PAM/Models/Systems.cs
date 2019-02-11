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

    [Table("RequestedSystems")]
    public class RequestedSystem
    {
        public int RequestId { get; set; }
        public Request Request { get; set; }

        public int SystemId { get; set; }
        public System System { get; set; }

        public bool InPortfolio { get; set; } = true;
        public bool RemoveAccess { get; set; } = false;

        public int? ProcessedById { get; set; }
        [ForeignKey(nameof(ProcessedById))]
        public Employee ProcessedBy { get; set; }
        public DateTime? ProcessedOn { get; set; }

        public int? ConfirmedById { get; set; }
        [ForeignKey(nameof(ConfirmedById))]
        public Employee ConfirmedBy { get; set; }
        public DateTime? ConfirmedOn { get; set; }

        public RequestedSystem() { }

        public RequestedSystem(int requestId, int systemId)
        {
            RequestId = requestId;
            SystemId = systemId;
        }
    }

    public enum SystemAccessStatus { Approved, Processed, Confirmed };

    [Table("SystemAccesses")]
    public class SystemAccess
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int SystemId { get; set; }
        public System System { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public bool RemoveAccess { get; set; }

        public SystemAccessStatus SystemAccessStatus { get; set; }
    }
}
