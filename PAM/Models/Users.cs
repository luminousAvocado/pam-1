using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using PAM.Extensions;

namespace PAM.Models
{
    [Table("Employees")]
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        public string Username { get; set; } // e.g. e123456

        [Required]
        public string Name { get; set; } // e.g. John Doe (e123456)

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Title { get; set; } // e.g. INFORMATION SYSTEMS ANALYST II
        public string Department { get; set; } // e.g. Probation #640
        public string Service { get; set; } // e.g. Probation - Support Services

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string Phone { get; set; }
        public string CellPhone { get; set; }

        public string SupervisorName { get; set; }

        public int? SupportUnitId { get; set; }
        public SupportUnit SupportUnit { get; set; }

        public bool IsAdmin { get; set; } = false;
        public bool IsApprover { get; set; } = false;

        public Employee() { }

        public Employee(ClaimsIdentity identity)
        {
            EmployeeId = Int32.Parse(identity.GetClaim("EmployeeId"));
            Username = identity.GetClaim(ClaimTypes.NameIdentifier);
            Name = identity.GetClaim(ClaimTypes.Name);
            FirstName = identity.GetClaim(ClaimTypes.GivenName);
            MiddleName = identity.GetClaim("MiddleName");
            LastName = identity.GetClaim(ClaimTypes.Surname);
            Email = identity.GetClaim(ClaimTypes.Email);

            Title = identity.GetClaim("Title");
            Department = identity.GetClaim("Department");
            Service = identity.GetClaim("Service");

            Address = identity.GetClaim(ClaimTypes.StreetAddress);
            City = identity.GetClaim(ClaimTypes.Locality);
            State = identity.GetClaim(ClaimTypes.StateOrProvince);
            Zip = identity.GetClaim(ClaimTypes.PostalCode);

            Phone = identity.GetClaim("Phone");
            CellPhone = identity.GetClaim(ClaimTypes.MobilePhone);

            SupervisorName = identity.GetClaim("Supervisor");

            var claim = identity.GetClaim("SupportUnitId");
            if (claim != null)
                SupportUnitId = Int32.Parse(claim);

            IsAdmin = identity.GetClaim("IsAdmin") != null ? true : false;
            IsApprover = identity.GetClaim("IsApprover") != null ? true : false;
        }

        public ClaimsIdentity ToClaimsIdentity()
        {
            var claims = new List<Claim>
            {
                new Claim("EmployeeId", EmployeeId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, Username),
                new Claim(ClaimTypes.Name, Name),
                new Claim(ClaimTypes.GivenName, FirstName),
                new Claim(ClaimTypes.Surname, LastName),
                new Claim(ClaimTypes.Email, Email)
            };

            if (MiddleName != null) claims.Add(new Claim("MiddleName", MiddleName));
            if (Title != null) claims.Add(new Claim("Title", Title));
            if (Department != null) claims.Add(new Claim("Department", Department));
            if (Service != null) claims.Add(new Claim("Service", Service));
            if (Address != null) claims.Add(new Claim(ClaimTypes.StreetAddress, Address));
            if (City != null) claims.Add(new Claim(ClaimTypes.Locality, City));
            if (State != null) claims.Add(new Claim(ClaimTypes.StateOrProvince, State));
            if (Zip != null) claims.Add(new Claim(ClaimTypes.PostalCode, Zip));
            if (Phone != null) claims.Add(new Claim("Phone", Phone));
            if (CellPhone != null) claims.Add(new Claim(ClaimTypes.MobilePhone, CellPhone));

            if (SupervisorName != null) claims.Add(new Claim("Supervisor", SupervisorName));

            if (SupportUnitId != null) claims.Add(new Claim("SupportUnitId", SupportUnitId.ToString()));

            if (IsAdmin) claims.Add(new Claim("IsAdmin", "true"));
            if (IsApprover) claims.Add(new Claim("IsApprover", "true"));

            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    [Table("Requesters")]
    public class Requester
    {
        public int RequesterId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public int? UnitId { get; set; }
        public Unit Unit { get; set; }

        public string Title { get; set; }
        public string Department { get; set; }
        public string Service { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        public string Phone { get; set; }
        public string CellPhone { get; set; }

        public string SupervisorName { get; set; }
    }
}
