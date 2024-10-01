using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class AMW_Calculator
    {
        public static async Task CalculateAMW(string solutionPath)
        {

            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);
            var namespaceName = string.Empty;

            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    var root = await document.GetSyntaxRootAsync();
                    if (root == null) continue;

                    var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                    var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
                  
                    foreach (var ns in namespaceDeclarations)
                    {
                        namespaceName = ns.Name.ToString();
                    }

                    foreach (var classDeclaration in classDeclarations)
                    {
                        var className = classDeclaration.Identifier.ToString();

                        var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

                        double totalStatements = 0;
                        var LOCNAMM = 0;
                        foreach (var method in methods)
                        {
                            try
                            {
                                LOCNAMM = method.Body.Statements.Count;
                            }
                            catch { }
                            totalStatements += LOCNAMM;
                        }

                        double AMW_type = methods.Any() ? totalStatements / methods.Count() : 0;

                        Console.WriteLine($"{project.Name};{namespaceName};{className};;;{AMW_type}");
                    }
                }
            }
        }
    }
}
