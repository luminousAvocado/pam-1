using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PAM.Models
{
    public class Bureau
    {
        public int BureauId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public int? BureauTypeId { get; set; }
        public int? DisplayOrder { get; set; }

        // navigational property
        public BureauType BureauType { get; set; }
    }

    public class BureauType
    {
        public int BureauTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string DisplayCode { get; set; }

        // navigational property
        public ICollection<Bureau> Bureaus { get; set; }
    }

}
