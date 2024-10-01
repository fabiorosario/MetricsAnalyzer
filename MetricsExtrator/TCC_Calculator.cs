using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace MetricsExtrator
{
    internal class TCC_Calculator
    {
        internal static double CalculateTCCForClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>()
                                .Where(m => !m.Modifiers.Any(SyntaxKind.PrivateKeyword) && !IsConstructor(m))
                                .ToList();

            var fields = classDeclaration.Members.OfType<FieldDeclarationSyntax>().ToList();

            if (methods.Count < 2) return 0;

            var methodFieldAccesses = new Dictionary<MethodDeclarationSyntax, HashSet<ISymbol>>();
            var directConnections = new Dictionary<MethodDeclarationSyntax, HashSet<MethodDeclarationSyntax>>();

            foreach (var method in methods)
            {
                var fieldSet = new HashSet<ISymbol>();
                var methodDescendants = method.DescendantNodes();

                foreach (var descendant in methodDescendants)
                {
                    if (descendant is IdentifierNameSyntax identifierName)
                    {
                        var symbolInfo = semanticModel.GetSymbolInfo(identifierName);
                        var symbol = symbolInfo.Symbol;

                        if (symbol != null && fields.SelectMany(f => f.Declaration.Variables)
                                                     .Any(v => semanticModel.GetDeclaredSymbol(v) == symbol))
                        {
                            fieldSet.Add(symbol);
                        }
                    }
                }

                methodFieldAccesses[method] = fieldSet;

                foreach (var otherMethod in methods)
                {
                    if (method != otherMethod && fieldSet.Intersect(methodFieldAccesses.GetValueOrDefault(otherMethod, new HashSet<ISymbol>())).Any())
                    {
                        if (!directConnections.ContainsKey(method))
                        {
                            directConnections[method] = new HashSet<MethodDeclarationSyntax>();
                        }
                        directConnections[method].Add(otherMethod);
                    }
                }
            }

            int directConnectionCount = directConnections.Sum(pair => pair.Value.Count);
            int totalPossibleConnections = methods.Count * (methods.Count - 1) / 2;

            return totalPossibleConnections > 0 ? (double)directConnectionCount / totalPossibleConnections : 0;
        }

        private static bool IsConstructor(MethodDeclarationSyntax method)
        {
            return method.Identifier.ValueText == method.Parent.DescendantNodes()
                        .OfType<ClassDeclarationSyntax>()
                        .FirstOrDefault()?.Identifier.ValueText;
        }
    }
}
