using MvcTemplate.Objects;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Tests
{
    public class TestView : BaseView
    {
        [StringLength(128)]
        public String? Title { get; set; }
    }
}
