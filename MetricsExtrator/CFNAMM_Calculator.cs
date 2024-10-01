using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    public class CFNAMM_Calculator
    {

        public static int CalculateMethodCFNAMM(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var calledMethods = new HashSet<IMethodSymbol>();
            var methodBody = method.Body;

            if (methodBody != null)
            {
                var invokedMethods = methodBody.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in invokedMethods)
                {
                    var invokedMethod = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                    if (invokedMethod != null && IsNonAccessorOrMutator(invokedMethod) && !IsDefaultConstructor(invokedMethod))
                    {
                        calledMethods.Add(invokedMethod);
                    }
                }
            }

            return calledMethods.Count;
        }

        public static int CalculateMethodCFNAMM(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
        {
            var calledMethods = new HashSet<IMethodSymbol>();
            var constructorBody = constructor.Body;

            if (constructorBody != null)
            {
                var invokedMethods = constructorBody.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in invokedMethods)
                {
                    var invokedMethod = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                    if (invokedMethod != null && IsNonAccessorOrMutator(invokedMethod) && !IsDefaultConstructor(invokedMethod))
                    {
                        calledMethods.Add(invokedMethod);
                    }
                }
            }

            return calledMethods.Count;
        }

        private static bool IsNonAccessorOrMutator(IMethodSymbol method)
        {
            return !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_");
        }

        private static bool IsDefaultConstructor(IMethodSymbol method)
        {
            return method.MethodKind == MethodKind.Constructor && method.Parameters.Length == 0;
        }

    }
}
