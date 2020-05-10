using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class AccountEditView : AView
    {
        [Required]
        [StringLength(32)]
        public String? Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public String? Email { get; set; }

        public Boolean IsLocked { get; set; }

        public Int64? RoleId { get; set; }
    }
}
