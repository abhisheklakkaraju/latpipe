using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Objects
{
    public class PermissionView : AView
    {
        [StringLength(64)]
        public String? Area { get; set; }

        [StringLength(64)]
        public String? Controller { get; set; }

        [StringLength(64)]
        public String? Action { get; set; }
    }
}
