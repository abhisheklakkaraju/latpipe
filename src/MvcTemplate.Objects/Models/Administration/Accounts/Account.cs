using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public class Account : AModel
    {
        [Required]
        [StringLength(32)]
        public String Username { get; set; }

        [Required]
        [StringLength(64)]
        public String Passhash { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public String Email { get; set; }

        public Boolean IsLocked { get; set; }

        [StringLength(36)]
        public String? RecoveryToken { get; set; }
        public DateTime? RecoveryTokenExpirationDate { get; set; }

        public Int64? RoleId { get; set; }
        public virtual Role? Role { get; set; }
    }
}
