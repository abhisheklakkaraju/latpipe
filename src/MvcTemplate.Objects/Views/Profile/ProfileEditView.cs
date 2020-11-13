using MvcTemplate.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class ProfileEditView : AView
    {
        [StringLength(32)]
        public String Username { get; set; }

        [NotTrimmed]
        [StringLength(32)]
        public String Password { get; set; }

        [NotTrimmed]
        [StringLength(32)]
        public String? NewPassword { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public String Email { get; set; }
    }
}
