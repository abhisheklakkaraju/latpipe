using Humanizer;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MvcTemplate.Web.Templates
{
    public class ModuleModel
    {
        public String Model { get; }
        public String Models { get; }
        public String ModelVarName { get; }
        public String ModelShortName { get; }

        public String View { get; }

        public String Service { get; }
        public String IService { get; }
        public String ServiceTests { get; }

        public String Validator { get; }
        public String IValidator { get; }
        public String ValidatorTests { get; }

        public String ControllerTestsNamespace { get; }
        public String ControllerNamespace { get; }
        public String ControllerTests { get; }
        public String Controller { get; }

        public String? Area { get; }

        public PropertyInfo[] Indexes { get; set; }
        public PropertyInfo[] ViewProperties { get; set; }
        public PropertyInfo[] ModelProperties { get; set; }
        public PropertyInfo[] AllViewProperties { get; set; }
        public PropertyInfo[] AllModelProperties { get; set; }
        public Dictionary<PropertyInfo, String?> Relations { get; set; }

        public ModuleModel(String model, String controller, String? area)
        {
            ModelShortName = Regex.Split(model, "(?=[A-Z])").Last();
            ModelVarName = ModelShortName.ToLower();
            Models = model.Pluralize();
            Model = model;

            View = $"{Model}View";

            ServiceTests = $"{Model}ServiceTests";
            IService = $"I{Model}Service";
            Service = $"{Model}Service";

            ValidatorTests = $"{Model}ValidatorTests";
            IValidator = $"I{Model}Validator";
            Validator = $"{Model}Validator";

            ControllerTestsNamespace = $"MvcTemplate.Controllers.{(String.IsNullOrWhiteSpace(area) ? "" : $".{area}")}Tests";
            ControllerNamespace = $"MvcTemplate.Controllers{(String.IsNullOrWhiteSpace(area) ? "" : $".{area}")}";
            ControllerTests = $"{controller}Tests";
            Controller = controller;

            Area = area;

            Type modelType = typeof(AModel).Assembly.GetType($"MvcTemplate.Objects.{Model}") ?? typeof(AModel);
            Type viewType = typeof(AView).Assembly.GetType($"MvcTemplate.Objects.{View}") ?? typeof(AModel);
            PropertyInfo[] modelProperties = modelType.GetProperties();

            AllModelProperties = modelProperties.Where(property => property.PropertyType.Namespace == "System").ToArray();
            ViewProperties = viewType.GetProperties().Where(property => property.DeclaringType?.Name == View).ToArray();
            ModelProperties = AllModelProperties.Where(property => property.DeclaringType?.Name == Model).ToArray();
            AllViewProperties = viewType.GetProperties();
            Indexes = ModelProperties
                .Where(property =>
                    ViewProperties.Any(prop => prop.Name == property.Name) &&
                    property.GetCustomAttribute<IndexAttribute>()?.IsUnique == true)
                .OrderByDescending(property => property.Name.Length)
                .ThenByDescending(property => property.Name)
                .ToArray();
            Relations = AllViewProperties
                .ToDictionary(
                    property => property,
                    property => modelProperties.SingleOrDefault(relation =>
                        property.Name.EndsWith("Id") &&
                        relation.PropertyType.Assembly == modelType.Assembly &&
                        relation.Name == property.Name[..^2])?.PropertyType.Name);
        }
    }
}
