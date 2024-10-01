using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    internal class CINT_Calculator
    {
        internal static IEnumerable<IMethodSymbol> CalculateCINTForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
        {
            var invokedMethods = method.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(invocation => semanticModel.GetSymbolInfo(invocation).Symbol)
            .OfType<IMethodSymbol>()
            .Where(invokedMethod => invokedMethod.ContainingType != null &&
                                    invokedMethod.ContainingType != semanticModel.GetDeclaredSymbol(classDeclaration));

            return invokedMethods;
        }
    }
}
