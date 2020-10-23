using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace MvcTemplate.Components.Mvc
{
    public class ValidationAdapterProvider : IValidationAttributeAdapterProvider
    {
        public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer? stringLocalizer)
        {
            return attribute switch
            {
                StringLengthAttribute stringLength => new StringLengthAdapter(stringLength),
                GreaterThanAttribute greaterThan => new GreaterThanAdapter(greaterThan),
                AcceptFilesAttribute acceptFiles => new AcceptFilesAdapter(acceptFiles),
                MinLengthAttribute minLength => new MinLengthAdapter(minLength),
                MaxLengthAttribute maxLength => new MaxLengthAdapter(maxLength),
                EmailAddressAttribute email => new EmailAddressAdapter(email),
                RequiredAttribute required => new RequiredAdapter(required),
                MaxValueAttribute maxValue => new MaxValueAdapter(maxValue),
                MinValueAttribute minValue => new MinValueAdapter(minValue),
                LessThanAttribute lessThan => new LessThanAdapter(lessThan),
                FileSizeAttribute fileSize => new FileSizeAdapter(fileSize),
                EqualToAttribute equalTo => new EqualToAdapter(equalTo),
                IntegerAttribute integer => new IntegerAdapter(integer),
                DigitsAttribute digits => new DigitsAdapter(digits),
                NumberAttribute number => new NumberAdapter(number),
                RangeAttribute range => new RangeAdapter(range),
                _ => null
            };
        }
    }
}
