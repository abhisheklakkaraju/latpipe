using Genny;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MvcTemplate.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MvcTemplate.Web.Templates
{
    [GennyModuleDescriptor("Default system module template")]
    public class Module : GennyModule
    {
        [GennyParameter(0, Required = true)]
        public String? Model { get; set; }

        [GennyParameter(1, Required = true)]
        public String? Controller { get; set; }

        [GennyParameter(2, Required = false)]
        public String? Area { get; set; }

        [GennySwitch("force", "f")]
        public Boolean Force { get; set; }

        public Module(IServiceProvider services)
            : base(services)
        {
        }

        public override void Run()
        {
            String path = $"{Area}/{Controller}".Trim('/');

            Dictionary<String, GennyScaffoldingResult> results = new()
            {
                { $"../MvcTemplate.Controllers/{path}/{Controller}.cs", Scaffold("Controllers/Controller") },
                { $"../../test/MvcTemplate.Tests/Unit/Controllers/{path}/{Controller}Tests.cs", Scaffold("Tests/ControllerTests") },

                { $"../MvcTemplate.Services/{path}/{Model}Service.cs", Scaffold("Services/Service") },
                { $"../MvcTemplate.Services/{path}/I{Model}Service.cs", Scaffold("Services/IService") },
                { $"../../test/MvcTemplate.Tests/Unit/Services/{path}/{Model}ServiceTests.cs", Scaffold("Tests/ServiceTests") },

                { $"../MvcTemplate.Validators/{path}/{Model}Validator.cs", Scaffold("Validators/Validator") },
                { $"../MvcTemplate.Validators/{path}/I{Model}Validator.cs", Scaffold("Validators/IValidator") },
                { $"../../test/MvcTemplate.Tests/Unit/Validators/{path}/{Model}ValidatorTests.cs", Scaffold("Tests/ValidatorTests") },

                { $"../MvcTemplate.Web/Views/{path}/Index.cshtml", Scaffold("Web/Index") },
                { $"../MvcTemplate.Web/Views/{path}/Create.cshtml", Scaffold("Web/Create") },
                { $"../MvcTemplate.Web/Views/{path}/Details.cshtml", Scaffold("Web/Details") },
                { $"../MvcTemplate.Web/Views/{path}/Edit.cshtml", Scaffold("Web/Edit") },
                { $"../MvcTemplate.Web/Views/{path}/Delete.cshtml", Scaffold("Web/Delete") },

                { $"../MvcTemplate.Web/Resources/Views/{path}/{Model}View.json", Scaffold("Resources/View") }
            };

            if (!File.Exists($"../MvcTemplate.Objects/Models/{path}/{Model}.cs") ||
                !File.Exists($"../MvcTemplate.Objects/Views/{path}/{Model}View.cs"))
            {
                results = new Dictionary<String, GennyScaffoldingResult>
                {
                    { $"../MvcTemplate.Objects/Models/{path}/{Model}.cs", Scaffold("Objects/Model") },
                    { $"../MvcTemplate.Objects/Views/{path}/{Model}View.cs", Scaffold("Objects/View") }
                };
            }

            if (results.Any(result => result.Value.Errors.Any()))
            {
                Dictionary<String, GennyScaffoldingResult> errors = new(results.Where(x => x.Value.Errors.Any()));

                Write(errors);

                Logger.WriteLine("");
                Logger.WriteLine("Scaffolding failed! Rolling back...", ConsoleColor.Red);
            }
            else
            {
                Logger.WriteLine("");

                if (Force)
                    Write(results);
                else
                    TryWrite(results);

                if (results.Count > 2)
                {
                    AddArea();
                    AddSiteMap();
                    AddPermissions();
                    AddViewImports();
                    AddObjectFactory();
                    AddPermissionTests();
                    AddTestingContextDrops();

                    AddResource("Page", "Headers", Model!, Model.Humanize());
                    AddResource("Page", "Headers", Model.Pluralize(), Model.Pluralize().Humanize());

                    AddResource("Page", "Titles", $"{Area}/{Controller}/Create".Trim('/'), $"{Model.Humanize()} creation");
                    AddResource("Page", "Titles", $"{Area}/{Controller}/Delete".Trim('/'), $"{Model.Humanize()} deletion");
                    AddResource("Page", "Titles", $"{Area}/{Controller}/Details".Trim('/'), $"{Model.Humanize()} details");
                    AddResource("Page", "Titles", $"{Area}/{Controller}/Index".Trim('/'), Model.Pluralize().Humanize());
                    AddResource("Page", "Titles", $"{Area}/{Controller}/Edit".Trim('/'), $"{Model.Humanize()} edit");

                    if (Area != null)
                        AddResource("Shared", "Areas", Area, Area.Humanize());
                    AddResource("Shared", "Controllers", $"{Area}/{Controller}".Trim('/'), Model.Pluralize().Humanize());

                    if (Area != null)
                        AddResource("SiteMap", "Titles", Area, Area.Humanize());
                    AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Create".Trim('/'), "Create");
                    AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Delete".Trim('/'), "Delete");
                    AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Details".Trim('/'), "Details");
                    AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Index".Trim('/'), Model.Pluralize().Humanize());
                    AddResource("SiteMap", "Titles", $"{Area}/{Controller}/Edit".Trim('/'), "Edit");

                    Logger.WriteLine("");
                    Logger.WriteLine("Scaffolded successfully!", ConsoleColor.Green);
                }
                else
                {
                    Logger.WriteLine("");
                    Logger.WriteLine("Scaffolded successfully! Write in model and view properties and rerun the scaffolding.", ConsoleColor.Green);
                }
            }
        }

        public override void ShowHelp()
        {
            Logger.WriteLine("Parameters:");
            Logger.WriteLine("    0 - Scaffolded model.");
            Logger.WriteLine("    1 - Scaffolded controller.");
            Logger.WriteLine("    2 - Scaffolded area (optional).");
        }

        private void AddArea()
        {
            if (Area == null)
                return;

            Logger.Write("../MvcTemplate.Controllers/Area.cs - ");

            String areas = File.ReadAllText("../MvcTemplate.Controllers/Area.cs");
            String newAreas = String.Join(",\n        ",
                Regex.Matches(areas, @"        (\w+),?")
                    .Select(match => match.Groups[1].Value)
                    .Append(Area)
                    .Distinct()
                    .OrderBy(name => name));

            areas = Regex.Replace(areas, @"    {\r?\n( +\w+,?\r?\n)+    }", $"    {{\n        {newAreas}\n    }}");

            File.WriteAllText("../MvcTemplate.Controllers/Area.cs", areas);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
        private void AddSiteMap()
        {
            Logger.Write("../MvcTemplate.Web/mvc.sitemap - ");

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
                XElement? parent = sitemap
                    .Descendants("siteMapNode")
                    .FirstOrDefault(node =>
                        node.Attribute("action") == null &&
                        node.Attribute("controller") == null &&
                        node.Attribute("area")?.Value == Area);

                if (parent == null)
                {
                    if (Area == null)
                    {
                        parent = sitemap.Descendants("siteMapNode").First();
                    }
                    else
                    {
                        parent = XElement.Parse($@"<siteMapNode menu=""true"" icon=""far fa-folder"" area=""{Area}"" />");
                        sitemap.Descendants("siteMapNode").First().Add(parent);
                    }
                }

                parent.Add(XElement.Parse(
                    $@"<siteMapNode menu=""true"" icon=""far fa-folder"" controller=""{Controller}"" action=""Index"">
                        <siteMapNode icon=""far fa-file"" action=""Create"" />
                        <siteMapNode icon=""fa fa-info"" action=""Details"" />
                        <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                        <siteMapNode icon=""fa fa-times"" action=""Delete"" />
                    </siteMapNode>"
                ));

                File.WriteAllText("mvc.sitemap", $"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n{sitemap.ToString().Replace("  ", "    ")}\n");

                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }
        private void AddPermissions()
        {
            Logger.Write("../MvcTemplate.Data/Migrations/Configuration.cs - ");

            String permissions = File.ReadAllText("../MvcTemplate.Data/Migrations/Configuration.cs");
            String newPermissions = String.Join(",\n",
                Regex.Matches(permissions, "new Permission {[^}]+}")
                    .Select(match => $"                {match.Value}")
                    .Append($@"                new Permission {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Create"" }}")
                    .Append($@"                new Permission {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Delete"" }}")
                    .Append($@"                new Permission {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Details"" }}")
                    .Append($@"                new Permission {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Edit"" }}")
                    .Append($@"                new Permission {{ Area = ""{Area}"", Controller = ""{Controller}"", Action = ""Index"" }}")
                    .Distinct()
                    .OrderBy(permission => permission));

            permissions = Regex.Replace(permissions, @"( +new Permission {[^}]+},?\r?\n+)+", $"{newPermissions}\n");

            File.WriteAllText("../MvcTemplate.Data/Migrations/Configuration.cs", permissions);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
        private void AddViewImports()
        {
            Logger.Write("../MvcTemplate.Web/Views/_ViewImports.cshtml - ");

            String imports = File.ReadAllText("../MvcTemplate.Web/Views/_ViewImports.cshtml");
            String newImports = String.Join("\n",
                Regex.Matches(imports, "@using (.+);")
                    .Select(match => match.Value)
                    .Append(Area == null ? "@using MvcTemplate.Controllers;" : $"@using MvcTemplate.Controllers.{Area};")
                    .Distinct()
                    .OrderBy(definition => definition.TrimEnd(';')));

            imports = Regex.Replace(imports, @"(@using (.+);\r?\n)+", $"{newImports}\n");

            File.WriteAllText("../MvcTemplate.Web/Views/_ViewImports.cshtml", imports);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
        private void AddObjectFactory()
        {
            Logger.Write("../../test/MvcTemplate.Tests/Helpers/ObjectsFactory.cs - ");

            ModuleModel model = new(Model!, Controller!, Area);
            SyntaxNode tree = CSharpSyntaxTree.ParseText(File.ReadAllText("../../test/MvcTemplate.Tests/Helpers/ObjectsFactory.cs")).GetRoot();

            if (tree.DescendantNodes().OfType<MethodDeclarationSyntax>().Any(method => method.Identifier.Text == $"Create{Model}"))
            {
                Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
            }
            else
            {
                String fakeView = FakeObjectCreation(model.View, model.AllViewProperties);
                String fakeModel = FakeObjectCreation(model.Model, model.ModelProperties);
                SyntaxNode last = tree.DescendantNodes().OfType<MethodDeclarationSyntax>().Last();
                SyntaxNode modelCreation = CSharpSyntaxTree.ParseText($"{fakeModel}{fakeView}\n").GetRoot();
                ClassDeclarationSyntax factory = tree.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

                tree = tree.ReplaceNode(factory, factory.InsertNodesAfter(last, modelCreation.ChildNodes()));

                File.WriteAllText("../../test/MvcTemplate.Tests/Helpers/ObjectsFactory.cs", tree.ToString());

                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }
        private void AddPermissionTests()
        {
            Logger.Write("../../test/MvcTemplate.Tests/Unit/Data/Migrations/ConfigurationTests.cs - ");

            String tests = File.ReadAllText("../../test/MvcTemplate.Tests/Unit/Data/Migrations/ConfigurationTests.cs");
            String newTests = String.Join("\n",
                Regex.Matches(tests, @"\[InlineData\(.*, ""\w+"", ""\w+""\)\]")
                    .Select(match => $"        {match.Value}")
                    .Append($@"        [InlineData(""{Area}"", ""{Controller}"", ""Create"")]")
                    .Append($@"        [InlineData(""{Area}"", ""{Controller}"", ""Delete"")]")
                    .Append($@"        [InlineData(""{Area}"", ""{Controller}"", ""Details"")]")
                    .Append($@"        [InlineData(""{Area}"", ""{Controller}"", ""Edit"")]")
                    .Append($@"        [InlineData(""{Area}"", ""{Controller}"", ""Index"")]")
                    .Distinct()
                    .OrderBy(test => test));

            tests = Regex.Replace(tests, @"( +\[InlineData\(.*, ""\w+"", ""\w+""\)\]\r?\n)+", $"{newTests}\n");

            File.WriteAllText("../../test/MvcTemplate.Tests/Unit/Data/Migrations/ConfigurationTests.cs", tests);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
        private void AddTestingContextDrops()
        {
            Logger.Write("../../test/MvcTemplate.Tests/Helpers/TestingContext.cs - ");

            String drops = File.ReadAllText("../../test/MvcTemplate.Tests/Helpers/TestingContext.cs");
            String newDrops = String.Join("\n",
                Regex.Matches(drops, @"context.RemoveRange\(context.Set<(.+)>\(\)\);")
                    .Select(match => $"            {match.Value}")
                    .Append($"            context.RemoveRange(context.Set<{Model}>());")
                    .Distinct()
                    .OrderByDescending(drop => drop.Length)
                    .ThenByDescending(drop => drop));

            drops = Regex.Replace(drops, @"( +context.RemoveRange\(context.Set<(.+)>\(\)\);\r?\n)+", $"{newDrops}\n");

            File.WriteAllText("../../test/MvcTemplate.Tests/Helpers/TestingContext.cs", drops);

            Logger.WriteLine("Succeeded", ConsoleColor.Green);
        }
        private void AddResource(String resource, String group, String key, String value)
        {
            Logger.Write($"../MvcTemplate.Web/Resources/Shared/{resource}.json - ");

            String page = File.ReadAllText($"Resources/Shared/{resource}.json");
            Dictionary<String, SortedDictionary<String, String>> resources = JsonSerializer.Deserialize<Dictionary<String, SortedDictionary<String, String>>>(page)!;

            if (resources[group].ContainsKey(key))
            {
                Logger.WriteLine("Already exists, skipping...", ConsoleColor.Yellow);
            }
            else
            {
                resources[group][key] = value;

                String text = Regex.Replace(JsonSerializer.Serialize(resources, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }), "(^ +)", "$1$1", RegexOptions.Multiline);

                File.WriteAllText($"Resources/Shared/{resource}.json", $"{text}\n");

                Logger.WriteLine("Succeeded", ConsoleColor.Green);
            }
        }

        private GennyScaffoldingResult Scaffold(String path)
        {
            return Scaffolder.Scaffold($"Templates/Module/{path}", new ModuleModel(Model!, Controller!, Area));
        }
        private String FakeObjectCreation(String name, PropertyInfo[] properties)
        {
            String creation = $"\n        public static {name} Create{name}(Int64 id)\n";
            creation += "        {\n";
            creation += $"            return new {name}\n";
            creation += "            {\n";

            creation += String.Join(",\n", properties
                .Where(property => property.Name != nameof(AModel.CreationDate))
                .OrderBy(property => property.Name.Length)
                .Select(property =>
                {
                    String set = $"                {property.Name} = ";

                    if (property.PropertyType == typeof(String))
                        return $"{set}$\"{property.Name}{{id}}\"";

                    if (typeof(Boolean?).IsAssignableFrom(property.PropertyType))
                        return $"{set}true";

                    if (typeof(DateTime?).IsAssignableFrom(property.PropertyType))
                        return $"{set}DateTime.Now.AddDays(id)";

                    return $"{set}id";
                })) + "\n";

            creation += "            };\n";
            creation += "        }";

            return creation;
        }
    }
}
