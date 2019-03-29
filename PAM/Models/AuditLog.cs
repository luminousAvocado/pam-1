using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PAM.Models
{
    public enum Action { Create, Edit, Delete, Submit, Approve, Deny };
    public enum ResourceType { Bureau, Location, System, Unit, Employee, Request, Review };

    [Table("AuditLogs")]
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int EmployeeId { get; set; }

        public Action Action {get; set;}

        public ResourceType ResourceType { get; set; }

        public int ResourceId { get; set; }

        // JSON of old/new values
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
