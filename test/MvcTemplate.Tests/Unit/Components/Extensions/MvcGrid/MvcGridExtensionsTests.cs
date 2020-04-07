using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using MvcTemplate.Components.Security;
using MvcTemplate.Resources;
using MvcTemplate.Tests;
using NonFactors.Mvc.Grid;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Encodings.Web;
using Xunit;

namespace MvcTemplate.Components.Extensions.Tests
{
    public class MvcGridExtensionsTests
    {
        private IGridColumnsOf<AllTypesView> columns;
        private IHtmlGrid<AllTypesView> html;

        public MvcGridExtensionsTests()
        {
            Grid<AllTypesView> grid = new Grid<AllTypesView>(Array.Empty<AllTypesView>());
            IHtmlHelper htmlHelper = HtmlHelperFactory.CreateHtmlHelper();
            html = new HtmlGrid<AllTypesView>(htmlHelper, grid);
            columns = new GridColumns<AllTypesView>(grid);
        }

        [Fact]
        public void AddAction_Unauthorized_Empty()
        {
            IGridColumn<AllTypesView, IHtmlContent> actual = columns.AddAction("Edit", "fa fa-pencil-alt");

            Assert.Empty(actual.ValueFor(new GridRow<AllTypesView>(new AllTypesView(), 0)).ToString());
            Assert.Empty(columns);
        }

        [Fact]
        public void AddAction_Authorized_Renders()
        {
            AllTypesView view = new AllTypesView();
            StringWriter writer = new StringWriter();
            IUrlHelper url = Substitute.For<IUrlHelper>();
            IUrlHelperFactory factory = Substitute.For<IUrlHelperFactory>();
            IAuthorization authorization = html.Grid.ViewContext!.HttpContext.RequestServices.GetService<IAuthorization>();

            html.Grid.ViewContext?.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(factory);
            authorization.IsGrantedFor(Arg.Any<Int64?>(), "//Details").Returns(true);
            factory.GetUrlHelper(html.Grid.ViewContext).Returns(url);
            url.Action(Arg.Any<UrlActionContext>()).Returns("/test");

            IGridColumn<AllTypesView, IHtmlContent> column = columns.AddAction("Details", "fa fa-info");
            column.ValueFor(new GridRow<AllTypesView>(view, 0)).WriteTo(writer, HtmlEncoder.Default);

            String expected = $"<a class=\"fa fa-info\" href=\"/test\" title=\"{Resource.ForAction("Details")}\"></a>";
            String actual = writer.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddAction_NoAuthorization_Renders()
        {
            AllTypesView view = new AllTypesView();
            StringWriter writer = new StringWriter();
            IUrlHelper url = Substitute.For<IUrlHelper>();
            IUrlHelperFactory factory = Substitute.For<IUrlHelperFactory>();

            html.Grid.ViewContext?.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(factory);
            html.Grid.ViewContext?.HttpContext.RequestServices.GetService(typeof(IAuthorization)).ReturnsNull();
            factory.GetUrlHelper(html.Grid.ViewContext).Returns(url);
            url.Action(Arg.Any<UrlActionContext>()).Returns("/test");

            IGridColumn<AllTypesView, IHtmlContent> column = columns.AddAction("Details", "fa fa-info");
            column.ValueFor(new GridRow<AllTypesView>(view, 0)).WriteTo(writer, HtmlEncoder.Default);

            String expected = $"<a class=\"fa fa-info\" href=\"/test\" title=\"{Resource.ForAction("Details")}\"></a>";
            String actual = writer.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddAction_NoId_Throws()
        {
            IUrlHelperFactory factory = Substitute.For<IUrlHelperFactory>();
            IGridColumnsOf<Object> gridColumns = new GridColumns<Object>(new Grid<Object>(Array.Empty<Object>()));

            html.Grid.ViewContext?.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(factory);
            html.Grid.ViewContext?.HttpContext.RequestServices.GetService(typeof(IAuthorization)).ReturnsNull();
            factory.GetUrlHelper(html.Grid.ViewContext).Returns(Substitute.For<IUrlHelper>());
            gridColumns.Grid.ViewContext = html.Grid.ViewContext;

            IGridColumn<Object, IHtmlContent> column = gridColumns.AddAction("Delete", "fa fa-times");

            String actual = Assert.Throws<Exception>(() => column.ValueFor(new GridRow<Object>(new Object(), 0))).Message;
            String expected = "Object type does not have an id.";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddAction_Column()
        {
            IUrlHelperFactory factory = Substitute.For<IUrlHelperFactory>();

            factory.GetUrlHelper(html.Grid.ViewContext).Returns(Substitute.For<IUrlHelper>());
            html.Grid.ViewContext?.HttpContext.RequestServices.GetService(typeof(IAuthorization)).ReturnsNull();
            html.Grid.ViewContext?.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(factory);

            IGridColumn<AllTypesView, IHtmlContent> actual = columns.AddAction("Edit", "fa fa-pencil-alt");

            Assert.Equal("action-cell edit", actual.CssClasses);
            Assert.Single(columns);
        }

        [Fact]
        public void AddDate_Column()
        {
            Expression<Func<AllTypesView, DateTime>> expression = (model) => model.DateTimeField;

            IGridColumn<AllTypesView, DateTime> actual = columns.AddDate(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal("date-time-field", actual.Name);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("{0:d}", actual.Format);
            Assert.Empty(actual.Title.ToString());
            Assert.Single(columns);
        }

        [Fact]
        public void AddDate_Nullable_Column()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            IGridColumn<AllTypesView, DateTime?> actual = columns.AddDate(expression);

            Assert.Equal("nullable-date-time-field", actual.Name);
            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Equal("{0:d}", actual.Format);
            Assert.Single(columns);
        }

        [Fact]
        public void AddBoolean_Column()
        {
            Expression<Func<AllTypesView, Boolean>> expression = (model) => model.BooleanField;

            IGridColumn<AllTypesView, Boolean> actual = columns.AddBoolean(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Equal("boolean-field", actual.Name);
            Assert.Empty(actual.Title.ToString());
            Assert.Single(columns);
        }

        [Fact]
        public void AddBoolean_True()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { BooleanField = true }, 0);
            IGridColumn<AllTypesView, Boolean> column = columns.AddBoolean(model => model.BooleanField);

            String? actual = column.ValueFor(row).ToString();
            String? expected = Resource.ForString("Yes");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddBoolean_False()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { BooleanField = false }, 0);
            IGridColumn<AllTypesView, Boolean> column = columns.AddBoolean(model => model.BooleanField);

            String? actual = column.ValueFor(row).ToString();
            String? expected = Resource.ForString("No");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddBoolean_Nullable_Column()
        {
            Expression<Func<AllTypesView, Boolean?>> expression = (model) => model.NullableBooleanField;

            IGridColumn<AllTypesView, Boolean?> actual = columns.AddBoolean(expression);

            Assert.Equal("nullable-boolean-field", actual.Name);
            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Single(columns);
        }

        [Fact]
        public void AddBoolean_Nullable()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = null }, 0);
            IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);

            Assert.Empty(column.ValueFor(row).ToString());
        }

        [Fact]
        public void AddBoolean_Nullable_True()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = true }, 0);
            IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);

            String? actual = column.ValueFor(row).ToString();
            String? expected = Resource.ForString("Yes");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddBoolean_Nullable_False()
        {
            GridRow<AllTypesView> row = new GridRow<AllTypesView>(new AllTypesView { NullableBooleanField = false }, 0);
            IGridColumn<AllTypesView, Boolean?> column = columns.AddBoolean(model => model.NullableBooleanField);

            String? actual = column.ValueFor(row).ToString();
            String? expected = Resource.ForString("No");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddDateTime_Column()
        {
            Expression<Func<AllTypesView, DateTime>> expression = (model) => model.DateTimeField;

            IGridColumn<AllTypesView, DateTime> actual = columns.AddDateTime(expression);

            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal("date-time-field", actual.Name);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Equal("{0:g}", actual.Format);
            Assert.Single(columns);
        }

        [Fact]
        public void AddDateTime_Nullable_Column()
        {
            Expression<Func<AllTypesView, DateTime?>> expression = (model) => model.NullableDateTimeField;

            IGridColumn<AllTypesView, DateTime?> actual = columns.AddDateTime(expression);

            Assert.Equal("nullable-date-time-field", actual.Name);
            Assert.Equal("text-center", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Equal("{0:g}", actual.Format);
            Assert.Single(columns);
        }

        [Fact]
        public void AddProperty_Column()
        {
            Expression<Func<AllTypesView, AllTypesView>> expression = (model) => model;

            IGridColumn<AllTypesView, AllTypesView> actual = columns.AddProperty(expression);

            Assert.Equal("text-left", actual.CssClasses);
            Assert.Equal(expression, actual.Expression);
            Assert.Empty(actual.Title.ToString());
            Assert.Empty(actual.Name);
            Assert.Single(columns);
        }

        [Fact]
        public void AddProperty_SetsColumnName()
        {
            Expression<Func<AllTypesView, SByte?>> expression = (model) => model.NullableSByteField;

            String actual = columns.AddProperty(expression).Name;
            String expected = "nullable-s-byte-field";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddProperty_SetsCssClassForEnum()
        {
            AssertCssClassFor(model => model.EnumField, "text-left");
        }

        [Fact]
        public void AddProperty_SetsCssClassForSByte()
        {
            AssertCssClassFor(model => model.SByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForByte()
        {
            AssertCssClassFor(model => model.ByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt16()
        {
            AssertCssClassFor(model => model.Int16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt16()
        {
            AssertCssClassFor(model => model.UInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt32()
        {
            AssertCssClassFor(model => model.Int32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt32()
        {
            AssertCssClassFor(model => model.UInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForInt64()
        {
            AssertCssClassFor(model => model.Int64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForUInt64()
        {
            AssertCssClassFor(model => model.UInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForSingle()
        {
            AssertCssClassFor(model => model.SingleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDouble()
        {
            AssertCssClassFor(model => model.DoubleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDecimal()
        {
            AssertCssClassFor(model => model.DecimalField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForBoolean()
        {
            AssertCssClassFor(model => model.BooleanField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForDateTime()
        {
            AssertCssClassFor(model => model.DateTimeField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableEnum()
        {
            AssertCssClassFor(model => model.NullableEnumField, "text-left");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableSByte()
        {
            AssertCssClassFor(model => model.NullableSByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableByte()
        {
            AssertCssClassFor(model => model.NullableByteField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt16()
        {
            AssertCssClassFor(model => model.NullableInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt16()
        {
            AssertCssClassFor(model => model.NullableUInt16Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt32()
        {
            AssertCssClassFor(model => model.NullableInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt32()
        {
            AssertCssClassFor(model => model.NullableUInt32Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableInt64()
        {
            AssertCssClassFor(model => model.NullableInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableUInt64()
        {
            AssertCssClassFor(model => model.NullableUInt64Field, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableSingle()
        {
            AssertCssClassFor(model => model.NullableSingleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDouble()
        {
            AssertCssClassFor(model => model.NullableDoubleField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDecimal()
        {
            AssertCssClassFor(model => model.NullableDecimalField, "text-right");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableBoolean()
        {
            AssertCssClassFor(model => model.NullableBooleanField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForNullableDateTime()
        {
            AssertCssClassFor(model => model.NullableDateTimeField, "text-center");
        }

        [Fact]
        public void AddProperty_SetsCssClassForOtherTypes()
        {
            AssertCssClassFor(model => model.StringField, "text-left");
        }

        [Theory]
        [InlineData("", "table-hover")]
        [InlineData(" ", "table-hover")]
        [InlineData(null, "table-hover")]
        [InlineData("test", "test table-hover")]
        [InlineData(" test", "test table-hover")]
        [InlineData("test ", "test  table-hover")]
        [InlineData(" test ", "test  table-hover")]
        public void ApplyDefaults_Values(String? cssClasses, String expectedClasses)
        {
            IGridColumn column = html.Grid.Columns.Add(model => model.ByteField);
            html.Grid.Attributes["class"] = cssClasses;
            column.Filter.IsEnabled = null;
            column.Sort.IsEnabled = null;

            IGrid actual = html.ApplyDefaults().Grid;

            Assert.Equal(Resource.ForString("NoDataFound"), actual.EmptyText);
            Assert.Equal(expectedClasses, html.Grid.Attributes["class"]);
            Assert.True(column.Filter.IsEnabled);
            Assert.True(column.Sort.IsEnabled);
            Assert.NotEmpty(actual.Columns);
        }

        private void AssertCssClassFor<TProperty>(Expression<Func<AllTypesView, TProperty>> property, String cssClasses)
        {
            IGridColumn<AllTypesView, TProperty> column = columns.AddProperty(property);

            String actual = column.CssClasses;
            String expected = cssClasses;

            Assert.Equal(expected, actual);
        }
    }
}
