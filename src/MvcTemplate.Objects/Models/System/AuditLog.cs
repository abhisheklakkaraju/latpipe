using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class AuditLog : AModel
    {
        public Int64? AccountId { get; set; }

        [Required]
        [StringLength(16)]
        public String Action { get; set; }

        [Required]
        [StringLength(64)]
        public String EntityName { get; set; }

        public Int64 EntityId { get; set; }

        [Required]
        public String Changes { get; set; }
    }
}
