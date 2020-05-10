using MvcTemplate.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class ProfileDeleteView : AView
    {
        [Required]
        [NotTrimmed]
        [StringLength(32)]
        public String? Password { get; set; }
    }
}
