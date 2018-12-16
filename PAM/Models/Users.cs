using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PAM.Models
{
    [Table("Employees")]
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string EmployeeNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Title { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }

        public bool IsAdmin { get; set; } = false;
        public bool IsApprover { get; set; } = false;
        public bool IsProcessor { get; set; } = false;

        public ICollection<SystemAccess> SystemAccesses { get; set; }

        public ClaimsIdentity ToClaimsIdentity()
        {
            var claims = new List<Claim>
            {
                new Claim("EmployeeId", EmployeeId.ToString()),
                new Claim("EmployeeNumber", EmployeeNumber),
                new Claim(ClaimTypes.NameIdentifier, Username),
                new Claim(ClaimTypes.GivenName, FirstName),
                new Claim(ClaimTypes.Surname, LastName),
                new Claim(ClaimTypes.Email, Email),
                new Claim("Title", Title),
                new Claim("Department", Department),
                new Claim("Phone", Phone)
            };
            if (IsAdmin) claims.Add(new Claim("IsAdmin", "true"));
            if (IsApprover) claims.Add(new Claim("IsApprover", "true"));
            if (IsProcessor) claims.Add(new Claim("IsProcessor", "true"));

            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    [Table("Requesters")]
    public class Requester
    {
        public int RequesterId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string EmployeeNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public int? BureauId { get; set; }
        public Bureau Bureau { get; set; }

        public int? UnitId { get; set; }
        public Unit Unit { get; set; }

        public string MiddleName { get; set; }

        public string PayrollTitle { get; set; }
        public string Department { get; set; }
        public string DepartmentCode { get; set; }

        public string WorkAddress { get; set; }
        public string WorkCity { get; set; }
        public string WorkState { get; set; }
        public string WorkZip { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }

        // derived properties
        [NotMapped]
        public string Name => $"{FirstName} {LastName}";
    }
}
