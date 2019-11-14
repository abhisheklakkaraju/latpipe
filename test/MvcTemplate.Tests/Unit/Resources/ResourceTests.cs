using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using Xunit;

namespace MvcTemplate.Resources.Tests
{
    public class ResourceTests
    {
        [Fact]
        public void Set_Same()
        {
            Object expected = Resource.Set("Test");
            Object actual = Resource.Set("Test");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void ForArea_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/Shared", "Areas", "Administration");
            String actual = Resource.ForArea("administration");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForArea_NotFound_Empty()
        {
            Assert.Empty(Resource.ForArea("Null"));
        }

        [Fact]
        public void ForAction_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/Shared", "Actions", "Create");
            String actual = Resource.ForAction("create");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForAction_NotFound_Empty()
        {
            Assert.Empty(Resource.ForAction("Null"));
        }

        [Fact]
        public void ForController_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/Shared", "Controllers", "AdministrationRoles");
            String actual = Resource.ForController("administrationroles");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForController_NotFound_Empty()
        {
            Assert.Empty(Resource.ForController("Null"));
        }

        [Fact]
        public void ForLookup_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/Lookup", "Titles", nameof(Role));
            String actual = Resource.ForLookup(nameof(Role).ToLower());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForLookup_NotFound_Empty()
        {
            Assert.Empty(Resource.ForLookup("Test"));
        }

        [Fact]
        public void ForString_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/Shared", "Strings", "All");
            String actual = Resource.ForString("all");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForString_Formats()
        {
            String expected = String.Format(ResourceFor("Shared/Shared", "Strings", "SystemError"), "test");
            String actual = Resource.ForString("SystemError", "test");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForString_NotFound_Empty()
        {
            Assert.Empty(Resource.ForString("Null"));
        }

        [Fact]
        public void ForHeader_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/Page", "Headers", "Account");
            String actual = Resource.ForHeader("account");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForHeader_NotFound_Empty()
        {
            Assert.Empty(Resource.ForHeader("Test"));
        }

        [Fact]
        public void ForPage_Path_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/Page", "Titles", "AdministrationRolesDetails");
            String actual = Resource.ForPage("administrationrolesdetails");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForPage_PathNotFound_Empty()
        {
            Assert.Empty(Resource.ForPage("Test"));
        }

        [Fact]
        public void ForPage_IsCaseInsensitive()
        {
            IDictionary<String, Object?> values = new Dictionary<String, Object?>
            {
                ["area"] = "administration",
                ["controller"] = "roles",
                ["action"] = "details"
            };

            String expected = ResourceFor("Shared/Page", "Titles", "AdministrationRolesDetails");
            String actual = Resource.ForPage(values);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void ForPage_WithoutArea(String? area)
        {
            IDictionary<String, Object?> values = new Dictionary<String, Object?>
            {
                ["controller"] = "profile",
                ["action"] = "edit",
                ["area"] = area
            };

            String expected = ResourceFor("Shared/Page", "Titles", "ProfileEdit");
            String actual = Resource.ForPage(values);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForPage_NotFound_Empty()
        {
            IDictionary<String, Object?> values = new Dictionary<String, Object?>
            {
                ["controller"] = null,
                ["action"] = null,
                ["area"] = null
            };

            Assert.Empty(Resource.ForPage(values));
        }

        [Fact]
        public void ForSiteMap_IsCaseInsensitive()
        {
            String expected = ResourceFor("Shared/SiteMap", "Titles", "AdministrationRolesIndex");
            String actual = Resource.ForSiteMap("administration", "roles", "index");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForSiteMap_WithoutControllerAndAction()
        {
            String expected = ResourceFor("Shared/SiteMap", "Titles", "Administration");
            String actual = Resource.ForSiteMap("administration", null, null);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForSiteMap_NotFound_Empty()
        {
            Assert.Empty(Resource.ForSiteMap("Test", "Test", "Test"));
        }

        [Fact]
        public void ForProperty_NotMemberLambdaExpression_ReturnNull()
        {
            Assert.Empty(Resource.ForProperty<TestView, String?>(view => view.ToString()));
        }

        [Fact]
        public void ForProperty_FromLambdaExpression()
        {
            String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Titles", nameof(AccountView.Username));
            String actual = Resource.ForProperty<AccountView, String?>(account => account.Username);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromLambdaExpressionRelation()
        {
            String expected = ResourceFor("Views/Administration/Roles/RoleView", "Titles", nameof(RoleView.Id));
            String actual = Resource.ForProperty<AccountEditView, Int32?>(account => account.RoleId);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_NotFoundLambdaExpression_Empty()
        {
            Assert.Empty(Resource.ForProperty<AccountView, Int32>(account => account.Id));
        }

        [Fact]
        public void ForProperty_NotFoundLambdaType_Empty()
        {
            Assert.Empty(Resource.ForProperty<TestView, String?>(test => test.Title));
        }

        [Fact]
        public void ForProperty_View()
        {
            String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Titles", nameof(AccountView.Username));
            String actual = Resource.ForProperty(nameof(AccountView), nameof(AccountView.Username));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_IsCaseInsensitive()
        {
            String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Titles", nameof(AccountView.Username));
            String actual = Resource.ForProperty(typeof(AccountView), nameof(AccountView.Username).ToLower());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromRelation()
        {
            String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Titles", nameof(AccountView.Username));
            String actual = Resource.ForProperty(typeof(Object), $"{nameof(Account)}{nameof(Account.Username)}");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromMultipleRelations()
        {
            String actual = Resource.ForProperty(typeof(RoleView), $"{nameof(Account)}{nameof(Role)}{nameof(Account)}{nameof(Account.Username)}");
            String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Titles", nameof(Account.Username));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_NotFoundProperty_Empty()
        {
            Assert.Empty(Resource.ForProperty(typeof(AccountView), nameof(AccountView.Id)));
        }

        [Fact]
        public void ForProperty_NotFoundTypeProperty_Empty()
        {
            Assert.Empty(Resource.ForProperty(typeof(TestView), nameof(TestView.Title)));
        }

        [Fact]
        public void ForProperty_NotMemberExpression_ReturnNull()
        {
            Expression<Func<TestView, String?>> lambda = (view) => view.ToString();

            Assert.Empty(Resource.ForProperty(lambda.Body));
        }

        [Fact]
        public void ForProperty_FromExpression()
        {
            Expression<Func<AccountView, String?>> lambda = (account) => account.Username;

            String expected = ResourceFor("Views/Administration/Accounts/AccountView", "Titles", nameof(AccountView.Username));
            String actual = Resource.ForProperty(lambda.Body);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_FromExpressionRelation()
        {
            Expression<Func<AccountEditView, Int32?>> lambda = (account) => account.RoleId;

            String expected = ResourceFor("Views/Administration/Roles/RoleView", "Titles", nameof(RoleView.Id));
            String actual = Resource.ForProperty(lambda.Body);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ForProperty_NotFoundExpression_Empty()
        {
            Expression<Func<AccountView, Int32>> lambda = (account) => account.Id;

            Assert.Empty(Resource.ForProperty(lambda.Body));
        }

        [Fact]
        public void ForProperty_NotFoundType_Empty()
        {
            Expression<Func<TestView, String?>> lambda = (test) => test.Title;

            Assert.Empty(Resource.ForProperty(lambda.Body));
        }

        private String ResourceFor(String path, String group, String key)
        {
            String resource = File.ReadAllText(Path.Combine("Resources", $"{path}.json"));

            return JsonSerializer.Deserialize<Dictionary<String, Dictionary<String, String?>>>(resource)[group][key] ?? "";
        }
    }
}
