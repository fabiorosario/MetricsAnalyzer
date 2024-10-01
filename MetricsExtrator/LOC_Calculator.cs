using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class LOC_Calculator
    {
        public static async Task CalculateLOC(string solutionPath)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            foreach (var project in solution.Projects)
            {
                var LOC_project = 0;
                var LOC_package = 0;
                var LOC_type = 0;
                var currentNamespace = string.Empty;

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
                            Console.WriteLine($"{project.Name};{currentNamespace};;;;;{LOC_package}");
                            LOC_package = 0;
                        }
                        currentNamespace = namespaceName;
                    }

                    foreach (var classDeclaration in classDeclarations)
                    {
                        var className = classDeclaration.Identifier.ToString();
                        var classLOC = classDeclaration.GetLocation().GetLineSpan().EndLinePosition.Line -
                                       classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

                        LOC_type = classLOC;
                        LOC_project += classLOC;
                        LOC_package += classLOC;

                        var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

                        if (methods != null && !methods.Any())
                        {
                            Console.WriteLine($"{project.Name};{currentNamespace};{className};;;{LOC_type}");
                        }
                        else
                        {
                            foreach (var method in methods)
                            {
                                var LOC_method = method.GetLocation().GetLineSpan().EndLinePosition.Line -
                                            method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

                                Console.WriteLine($"{project.Name};{currentNamespace};{className};{method.Identifier};{LOC_method};{LOC_type}");
                            }
                        }
                    }
                }
                Console.WriteLine($"{project.Name};;;;;;;{LOC_project}");
            }
        }
    }
}

