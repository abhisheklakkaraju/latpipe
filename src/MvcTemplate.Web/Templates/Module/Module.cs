using Genny;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MvcTemplate.Web.Templates
{
    [GennyModuleDescriptor("Default system module template")]
    public class Module : GennyModule
    {
        [GennyParameter(0, Required = true)]
        public String Model { get; set; }

        [GennyParameter(1, Required = true)]
        public String Controller { get; set; }

        [GennyParameter(2, Required = false)]
        public String Area { get; set; }

        public Module(IServiceProvider services)
            : base(services)
        {
        }

        public override void Run()
        {
            String path = $"{(Area != null ? $"{Area}/" : "")}{Controller}";
            Dictionary<String, GennyScaffoldingResult> results = new Dictionary<String, GennyScaffoldingResult>();

            results.Add($"../MvcTemplate.Controllers/{path}/{Controller}Controller.cs", Scaffold("Controllers/Controller"));
            results.Add($"../../test/MvcTemplate.Tests/Unit/Controllers/{path}/{Controller}ControllerTests.cs", Scaffold("Tests/ControllerTests"));

            results.Add($"../MvcTemplate.Objects/Models/{path}/{Model}.cs", Scaffold("Objects/Model"));
            results.Add($"../MvcTemplate.Objects/Views/{path}/{Model}View.cs", Scaffold("Objects/View"));

            results.Add($"../MvcTemplate.Services/{path}/{Model}Service.cs", Scaffold("Services/Service"));
            results.Add($"../MvcTemplate.Services/{path}/I{Model}Service.cs", Scaffold("Services/IService"));
            results.Add($"../../test/MvcTemplate.Tests/Unit/Services/{path}/{Model}ServiceTests.cs", Scaffold("Tests/ServiceTests"));

            results.Add($"../MvcTemplate.Validators/{path}/{Model}Validator.cs", Scaffold("Validators/Validator"));
            results.Add($"../MvcTemplate.Validators/{path}/I{Model}Validator.cs", Scaffold("Validators/IValidator"));
            results.Add($"../../test/MvcTemplate.Tests/Unit/Validators/{path}/{Model}ValidatorTests.cs", Scaffold("Tests/ValidatorTests"));

            results.Add($"../MvcTemplate.Web/Views/{path}/Index.cshtml", Scaffold("Web/Index"));
            results.Add($"../MvcTemplate.Web/Views/{path}/Create.cshtml", Scaffold("Web/Create"));
            results.Add($"../MvcTemplate.Web/Views/{path}/Details.cshtml", Scaffold("Web/Details"));
            results.Add($"../MvcTemplate.Web/Views/{path}/Edit.cshtml", Scaffold("Web/Edit"));
            results.Add($"../MvcTemplate.Web/Views/{path}/Delete.cshtml", Scaffold("Web/Delete"));

            results.Add($"../MvcTemplate.Web/Resources/Views/{path}/{Model}View.json", Scaffold("Resources/View"));

            if (results.Any(result => result.Value.Errors.Any()))
            {
                Dictionary<String, GennyScaffoldingResult> errors = new Dictionary<String, GennyScaffoldingResult>(results.Where(x => x.Value.Errors.Any()));

                Write(errors);

                Logger.WriteLine("");
                Logger.WriteLine("Scaffolding failed! Rolling back...", ConsoleColor.Red);
            }
            else
            {
                Logger.WriteLine("");

                TryWrite(results);

                AddSiteMap();
                AddPermissions();
                AddObjectFactory();

                AddPermissionTests("Index");
                AddPermissionTests("Create");
                AddPermissionTests("Details");
                AddPermissionTests("Edit");
                AddPermissionTests("Delete");

                AddResource("Page", "Headers", Model, Model.Humanize());
                AddResource("Page", "Headers", Model.Pluralize(), Model.Pluralize().Humanize());

                AddResource("Page", "Titles", $"{Area}{Controller}Create", $"{Model.Humanize()} creation");
                AddResource("Page", "Titles", $"{Area}{Controller}Delete", $"{Model.Humanize()} deletion");
                AddResource("Page", "Titles", $"{Area}{Controller}Details", $"{Model.Humanize()} details");
                AddResource("Page", "Titles", $"{Area}{Controller}Index", Model.Pluralize().Humanize());
                AddResource("Page", "Titles", $"{Area}{Controller}Edit", $"{Model.Humanize()} edit");

                AddResource("Shared", "Areas", Area, Area?.Humanize());
                AddResource("Shared", "Controllers", $"{Area}{Controller}", Model.Pluralize().Humanize());

                AddResource("SiteMap", "Titles", Area, Area?.Humanize());
                AddResource("SiteMap", "Titles", $"{Area}{Controller}Create", "Create");
                AddResource("SiteMap", "Titles", $"{Area}{Controller}Delete", "Delete");
                AddResource("SiteMap", "Titles", $"{Area}{Controller}Details", "Details");
                AddResource("SiteMap", "Titles", $"{Area}{Controller}Index", Model.Pluralize().Humanize());
                AddResource("SiteMap", "Titles", $"{Area}{Controller}Edit", "Edit");

                Logger.WriteLine("");
                Logger.WriteLine("Scaffolded successfully!", ConsoleColor.Green);
            }
        }

        public override void ShowHelp()
        {
            Logger.WriteLine("Parameters:");
            Logger.WriteLine("    1 - Scaffolded model.");
            Logger.WriteLine("    2 - Scaffolded controller.");
            Logger.WriteLine("    3 - Scaffolded area (optional).");
        }

        private void AddSiteMap()
        {
            Logger.Write($"../MvcTemplate.Web/mvc.sitemap - ");

            XElement sitemap = XElement.Parse(File.ReadAllText("mvc.sitemap"));
            Boolean isDefined = sitemap
                .Descendants("siteMapNode")
                .Any(node =>
                    node.Attribute("action")?.Value == "Index" &&
                    node.Parent?.Attribute("area")?.Value == Area &&
                    node.Attribute("controller")?.Value == Controller);

            if (isDefined)
            {
                Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
            }
            else
            {
                XElement areaNode = sitemap
                    .Descendants("siteMapNode")
                    .FirstOrDefault(node =>
                        node.Attribute("action") == null &&
                        node.Attribute("controller") == null &&
                        node.Attribute("area")?.Value == Area);

                if (areaNode == null)
                {
                    areaNode = XElement.Parse($@"<siteMapNode menu=""true"" icon=""far fa-folder"" area=""{Area}"" />");

                    sitemap.Descendants("siteMapNode").First().Add(areaNode);
                }

                areaNode.Add(XElement.Parse(
                    $@"<siteMapNode menu=""true"" icon=""far fa-folder"" controller=""{Controller}"" action=""Index"">
                        <siteMapNode icon=""far fa-file"" action=""Create"" />
                        <siteMapNode icon=""fa fa-info"" action=""Details"" />
                        <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                        <siteMapNode icon=""fa fa-times"" action=""Delete"" />
                    </siteMapNode>"
                ));

                File.WriteAllText("mvc.sitemap", $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{sitemap.ToString().Replace("  ", "    ")}{Environment.NewLine}");

                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }
        private void AddPermissions()
        {
            Logger.Write("../MvcTemplate.Data/Migrations/Configuration.cs - ");

            String configuration = File.ReadAllText("../MvcTemplate.Data/Migrations/Configuration.cs");
            MatchCollection matches = Regex.Matches(configuration, "new Permission {[^}]+}");

            if (matches.Count > 0)
            {
                String newPermissions = String.Join($",{Environment.NewLine}                ",
                    new[] { "Index", "Create", "Edit", "Details", "Delete" }
                        .Select(action => $"new Permission {{ Area = \"{Area}\", Controller = \"{Controller}\", Action = \"{action}\" }}")
                        .Where(permission => !configuration.Contains(permission)));

                if (newPermissions.Length > 0)
                {
                    Int32 lastPermission = matches.Last().Index + matches.Last().Length;
                    newPermissions = $",{Environment.NewLine}{Environment.NewLine}                {newPermissions}";

                    configuration = configuration.Insert(lastPermission, newPermissions);

                    File.WriteAllText("../MvcTemplate.Data/Migrations/Configuration.cs", configuration);

                    Logger.WriteLine("Success", ConsoleColor.Green);
                }
                else
                {
                    Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
                }
            }
            else
            {
                Logger.WriteLine("Missing, skipping", ConsoleColor.Yellow);
            }
        }
        private void AddObjectFactory()
        {
            Logger.Write($"../../test/MvcTemplate.Tests/Helpers/ObjectsFactory.cs - ");

            ModuleModel model = new ModuleModel(Model, Controller, Area);
            SyntaxNode tree = CSharpSyntaxTree.ParseText(File.ReadAllText("../../test/MvcTemplate.Tests/Helpers/ObjectsFactory.cs")).GetRoot();

            if (tree.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(method => method.Identifier.Text == $"Create{Model}"))
            {
                Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
            }
            else
            {
                String fakeView = FakeObjectCreation(model.View, model.AllViewProperties);
                String fakeModel = FakeObjectCreation(model.Model, model.AllModelProperties);
                SyntaxNode last = tree.DescendantNodes().OfType<MethodDeclarationSyntax>().Last();
                ClassDeclarationSyntax factory = tree.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
                SyntaxNode modelCreation = CSharpSyntaxTree.ParseText($"{fakeModel}{fakeView}{Environment.NewLine}").GetRoot();

                tree = tree.ReplaceNode(factory, factory.InsertNodesAfter(last, modelCreation.ChildNodes()));

                File.WriteAllText("../../test/MvcTemplate.Tests/Helpers/ObjectsFactory.cs", tree.ToString());

                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }
        private void AddPermissionTests(String action)
        {
            Logger.Write($"../../test/MvcTemplate.Tests/Unit/Data/Migrations/InitialDataTests.cs - ");

            String testData = $"[InlineData(\"{Area}\", \"{Controller}\", \"{action}\")]";
            String tests = File.ReadAllText("../../test/MvcTemplate.Tests/Unit/Data/Migrations/InitialDataTests.cs");

            if (tests.Contains(testData))
            {
                Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
            }
            else
            {
                testData += $"{Environment.NewLine}        ";
                tests = tests.Insert(tests.IndexOf("public void PermissionsTable_HasPermission"), testData);

                File.WriteAllText("../../test/MvcTemplate.Tests/Unit/Data/Migrations/InitialDataTests.cs", tests);

                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }
        private void AddResource(String resource, String group, String key, String value)
        {
            Logger.Write($"../MvcTemplate.Web/Resources/Shared/{resource}.json - ");

            String page = File.ReadAllText($"Resources/Shared/{resource}.json");
            Dictionary<String, SortedDictionary<String, String>> resources = JsonSerializer.Deserialize<Dictionary<String, SortedDictionary<String, String>>>(page);

            if (key != null && resources[group].ContainsKey(key))
            {
                Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
            }
            else if (key != null)
            {
                resources[group][key] = value;

                String text = Regex.Replace(JsonSerializer.Serialize(resources, new JsonSerializerOptions
                {
                    WriteIndented = true
                }), "(^ +)", "$1$1");

                File.WriteAllText($"Resources/Shared/{resource}.json", $"{text}{Environment.NewLine}");

                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }

        private GennyScaffoldingResult Scaffold(String path)
        {
            return Scaffolder.Scaffold($"Templates/Module/{path}", new ModuleModel(Model, Controller, Area));
        }
        private String FakeObjectCreation(String name, PropertyInfo[] properties)
        {
            String creation = $"\n        public static {name} Create{name}(Int32 id = 1)\n";
            creation += "        {\n";
            creation += $"            return new {name}\n";
            creation += "            {\n";

            creation += String.Join(",\n", properties
                .Where(property => property.Name != "CreationDate")
                .OrderBy(property => property.Name.Length)
                .Select(property =>
                {
                    String set = $"                 {property.Name} = ";

                    if (property.PropertyType == typeof(String))
                        return $"{set}$\"{property.Name}{{id}}\"";
                    else if (typeof(Boolean?).IsAssignableFrom(property.PropertyType))
                        return $"{set}true";
                    else if (typeof(DateTime?).IsAssignableFrom(property.PropertyType))
                        return $"{set}DateTime.Now.AddDays(id)";
                    else
                        return $"{set}id";
                })) + "\n";

            creation += "            };\n";
            creation += "        }";

            return creation.Replace("\n", Environment.NewLine);
        }
    }
}
