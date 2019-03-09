namespace PAM.Models
{
    public class EmployeeBindingModel
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Title { get; set; }
        public string Department { get; set; }
        public string Service { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string Phone { get; set; }
        public string CellPhone { get; set; }

        public bool IsAdmin { get; set; } = false;
        public bool IsApprover { get; set; } = false;
    }
}
