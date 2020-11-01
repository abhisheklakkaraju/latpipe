using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MvcTemplate.Resources;
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class ModelMessagesProviderTests
    {
        private DefaultModelBindingMessageProvider messages;

        public ModelMessagesProviderTests()
        {
            messages = new DefaultModelBindingMessageProvider();
            ModelMessagesProvider.Set(messages);
        }

        [Fact]
        public void ModelMessagesProvider_SetsAttemptedValueIsInvalidAccessor()
        {
            String actual = messages.AttemptedValueIsInvalidAccessor("Test", "Property");
            String expected = Validation.For("InvalidField", "Property");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ModelMessagesProvider_SetsUnknownValueIsInvalidAccessor()
        {
            String actual = messages.UnknownValueIsInvalidAccessor("Property");
            String expected = Validation.For("InvalidField", "Property");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ModelMessagesProvider_SetsMissingBindRequiredValueAccessor()
        {
            String actual = messages.MissingBindRequiredValueAccessor("Property");
            String expected = Validation.For("Required", "Property");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ModelMessagesProvider_SetsValueMustNotBeNullAccessor()
        {
            String actual = messages.ValueMustNotBeNullAccessor("Property");
            String expected = Validation.For("Required", "Property");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ModelMessagesProvider_ValueIsInvalidAccessor()
        {
            String expected = Validation.For("InvalidValue", "Value");
            String actual = messages.ValueIsInvalidAccessor("Value");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ModelMessagesProvider_SetsValueMustBeANumberAccessor()
        {
            String actual = messages.ValueMustBeANumberAccessor("Property");
            String expected = Validation.For("Number", "Property");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ModelMessagesProvider_SetsMissingKeyOrValueAccessor()
        {
            String actual = messages.MissingKeyOrValueAccessor();
            String expected = Validation.For("RequiredValue");

            Assert.Equal(expected, actual);
        }
    }
}
