using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    internal class LCOM5_Calculator
    {
        internal static double CalculateLCOM5ForClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();
            var fields = classDeclaration.Members.OfType<FieldDeclarationSyntax>();

            if (!methods.Any()) return 0;

            var fieldSet = new HashSet<ISymbol>();

            foreach (var field in fields)
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(variable);
                    if (symbol != null)
                    {
                        fieldSet.Add(symbol);
                    }
                }
            }

            if (!fieldSet.Any()) return 0;

            double sumOfIntersections = 0;

            foreach (var method in methods)
            {
                var methodSymbol = semanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
                if (methodSymbol == null) continue;

                var methodFieldSet = new HashSet<ISymbol>();

                var methodDescendants = method.DescendantNodes();

                foreach (var descendant in methodDescendants)
                {
                    if (descendant is IdentifierNameSyntax identifierName)
                    {
                        var symbolInfo = semanticModel.GetSymbolInfo(identifierName);
                        var symbol = symbolInfo.Symbol;

                        if (symbol != null && fieldSet.Contains(symbol))
                        {
                            methodFieldSet.Add(symbol);
                        }
                    }
                }

                double intersectionCount = methodFieldSet.Count;
                double fieldCount = fieldSet.Count;

                if (fieldCount != 0)
                {
                    sumOfIntersections += (intersectionCount / fieldCount);
                }
            }

            double n = methods.Count();
            double lcom5 = 1 - (sumOfIntersections / (n - 1));

            if (double.IsNaN(lcom5) || n == 1 || lcom5 < 0) return 0;

            return lcom5;
        }
    }
}
