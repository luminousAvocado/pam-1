using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAM.Models
{
    [Table("Systems")]
    public class System
    {
        public int SystemId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Owner { get; set; }

        public bool Retired { get; set; }
    }

    [Table("UnitSystems")]
    public class UnitSystem
    {
        public int UnitId { get; set; }
        public int SystemId { get; set; }

        // navigation properties
        public Unit Unit { get; set; }
        public System System { get; set; }
    }
}
