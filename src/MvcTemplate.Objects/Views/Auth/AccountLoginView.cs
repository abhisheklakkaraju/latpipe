using MvcTemplate.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class AccountLoginView : AView
    {
        [StringLength(32)]
        public String Username { get; set; }

        [NotTrimmed]
        [StringLength(32)]
        public String Password { get; set; }
    }
}
