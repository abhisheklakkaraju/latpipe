using MvcTemplate.Data;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Tests;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Validators.Tests
{
    public class RoleValidatorTests : IDisposable
    {
        private RoleValidator validator;
        private TestingContext context;
        private Role role;

        public RoleValidatorTests()
        {
            context = new TestingContext();
            validator = new RoleValidator(new UnitOfWork(new TestingContext(context)));

            context.Drop().Add(role = ObjectsFactory.CreateRole(0));
            context.SaveChanges();
        }
        public void Dispose()
        {
            context.Dispose();
            validator.Dispose();
        }

        [Fact]
        public void CanCreate_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanCreate(ObjectsFactory.CreateRoleView(role.Id + 1)));
            Assert.Single(validator.ModelState);
        }

        [Fact]
        public void CanCreate_UsedTitle_ReturnsFalse()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);
            view.Title = role.Title.ToLower();
            view.Id = role.Id;

            Boolean canCreate = validator.CanCreate(view);

            Assert.False(canCreate);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<RoleView>("UniqueTitle"), validator.ModelState[nameof(RoleView.Title)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanCreate_ValidRole()
        {
            Assert.True(validator.CanCreate(ObjectsFactory.CreateRoleView(role.Id + 1)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanEdit_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanEdit(ObjectsFactory.CreateRoleView(role.Id + 1)));
            Assert.Single(validator.ModelState);
        }

        [Fact]
        public void CanEdit_UsedTitle_ReturnsFalse()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);
            view.Title = role.Title.ToLower();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<RoleView>("UniqueTitle"), validator.ModelState[nameof(RoleView.Title)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_ValidRole()
        {
            Assert.True(validator.CanEdit(ObjectsFactory.CreateRoleView(role.Id + 1)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }
    }
}
