using MvcTemplate.Objects;
using System;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Tests
{
    public class TestModel : AModel
    {
        [StringLength(128)]
        public String? Title { get; set; }
    }
}
