using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAM.Models
{
    [Table("Locations")]
    public class Location
    {
        public int LocationId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Zip { get; set; }
    }

    [Table("BureauTypes")]
    public class BureauType
    {
        public int BureauTypeId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string DisplayCode { get; set; }
    }

    [Table("Bureaus")]
    public class Bureau
    {
        public int BureauId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public int? BureauTypeId { get; set; }
        public int? DisplayOrder { get; set; }

        // navigation property
        public BureauType BureauType { get; set; }
    }

    [Table("UnitTypes")]
    public class UnitType
    {
        public int UnitTypeId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string DisplayCode { get; set; }

        public string Description { get; set; }

        public int? DisplayOrder { get; set; }
    }

    [Table("Units")]
    public class Unit
    {
        public int UnitId { get; set; }

        [Required]
        public string Name { get; set; }

        public int BureauId { get; set; }
        public int? UnitTypeId { get; set; }
        public int? ParentId { get; set; }

        public int? DisplayOrder { get; set; }

        // navigation properties
        public Bureau Bureau { get; set; }
        public UnitType UnitType { get; set; }
        public Unit Parent { get; set; }
        [InverseProperty("Parent")]
        public ICollection<Unit> Children { get; set; }
        public ICollection<UnitSystem> Systems { get; set; }
    }
}
