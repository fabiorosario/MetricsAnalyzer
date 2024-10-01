using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class NOM_Calculator
    {
        public static async Task CalculateNOM(string solutionPath)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            foreach (var project in solution.Projects)
            {
                var currentNamespace = string.Empty;
                var NOM_project = 0;
                var NOM_package = 0;

                foreach (var document in project.Documents)
                {
                    var root = await document.GetSyntaxRootAsync();
                    if (root == null) continue;

                    var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
                    var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                    foreach (var ns in namespaceDeclarations)
                    {
                        var namespaceName = ns.Name.ToString();

                        if (!string.IsNullOrEmpty(currentNamespace) && currentNamespace != namespaceName)
                        {
                            Console.WriteLine($"{project.Name};{currentNamespace};;;;;{NOM_package}");
                            NOM_package = 0;
                        }
                        currentNamespace = namespaceName;
                    }

                    foreach (var classDeclaration in classDeclarations)
                    {
                        var className = classDeclaration.Identifier.ToString();
                        var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

                        var NOM_class = methods.Count();
                        NOM_package += NOM_class;
                        NOM_project += NOM_class;

                        Console.WriteLine($"{project.Name};{currentNamespace};{className};{NOM_class}");
                    }
                }
                Console.WriteLine($"{project.Name};;;;;;;{NOM_project}");
            }
        }
    }
}

