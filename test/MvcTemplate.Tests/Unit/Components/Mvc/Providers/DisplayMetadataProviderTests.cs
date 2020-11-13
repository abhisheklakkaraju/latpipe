using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class DisplayMetadataProviderTests
    {
        [Fact]
        public void CreateDisplayMetadata_SetsDisplayName()
        {
            DisplayMetadataProvider provider = new();
            DisplayMetadataProviderContext context = new(
                ModelMetadataIdentity.ForProperty(typeof(RoleView).GetProperty(nameof(RoleView.Title))!, typeof(String), typeof(RoleView)),
                ModelAttributes.GetAttributesForType(typeof(RoleView)));

            provider.CreateDisplayMetadata(context);

            String expected = Resource.ForProperty(typeof(RoleView), nameof(RoleView.Title));
            String actual = context.DisplayMetadata.DisplayName();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateDisplayMetadata_NullContainerType_DoesNotSetDisplayName()
        {
            DisplayMetadataProvider provider = new();
            DisplayMetadataProviderContext context = new(
                   ModelMetadataIdentity.ForType(typeof(RoleView)),
                   ModelAttributes.GetAttributesForType(typeof(RoleView)));

            provider.CreateDisplayMetadata(context);

            Assert.Null(context.DisplayMetadata.DisplayName);
        }
    }
}
