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
        public bool Retired { get; set; } = false;
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
