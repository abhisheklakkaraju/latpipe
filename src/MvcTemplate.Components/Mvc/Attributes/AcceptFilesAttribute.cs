using Microsoft.AspNetCore.Http;
using MvcTemplate.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MvcTemplate.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class AcceptFilesAttribute : ValidationAttribute
    {
        public String Extensions { get; }

        public AcceptFilesAttribute(String extensions)
            : base(() => Validation.For("AcceptFiles"))
        {
            Extensions = extensions;
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, Extensions);
        }
        public override Boolean IsValid(Object? value)
        {
            if (value == null)
                return true;

            IEnumerable<IFormFile>? files = value is IFormFile formFile ? new[] { formFile } : value as IEnumerable<IFormFile>;

            return files?.All(file => Extensions.Split(',').Any(ext => file.FileName.EndsWith(ext))) == true;
        }
    }
}
