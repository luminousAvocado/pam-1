using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAM.Models
{
    public enum LogActionType { Create, Update, Remove, Submit, Approve, Deny };
    public enum LogResourceType { Bureau, Location, System, Unit, ProcessingUnit, User, Request };

    [Table("AuditLog")]
    public class AuditLogEntry
    {
        public int AuditLogEntryId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public LogActionType ActionType { get; set; }
        public LogResourceType ResourceType { get; set; }
        public int ResourceId { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public AuditLogEntry(int employeeId, LogActionType actionType, LogResourceType resourceType, int resourceId, string message,
            string oldValue = null, string newValue = null)
        {
            EmployeeId = employeeId;
            ActionType = actionType;
            ResourceType = resourceType;
            ResourceId = resourceId;
            Message = message;
            Timestamp = DateTime.Now;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
