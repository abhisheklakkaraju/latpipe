using MvcTemplate.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class AccountResetView : AView
    {
        [Required]
        [StringLength(36)]
        public String? Token { get; set; }

        [Required]
        [NotTrimmed]
        [StringLength(32)]
        public String? NewPassword { get; set; }
    }
}
