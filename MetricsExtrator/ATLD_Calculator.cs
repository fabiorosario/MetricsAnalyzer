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
    public class ATLD_Calculator
    {
        public static int CalculateMethodATLD(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var usedVariables = new HashSet<ISymbol>();
            var methodBody = method.Body;

            if (methodBody != null)
            {
                var variableAccesses = methodBody.DescendantNodes().OfType<IdentifierNameSyntax>();
                foreach (var variableAccess in variableAccesses)
                {
                    try
                    {
                        var symbol = semanticModel.GetSymbolInfo(variableAccess).Symbol;
                        if (symbol != null && IsLocalToSystem(symbol, semanticModel))
                        {
                            usedVariables.Add(symbol);
                        }
                    }
                    catch { }
                }

                var invokedMethods = methodBody.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in invokedMethods)
                {
                    var invokedMethod = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                    if (invokedMethod != null && IsLocalToSystem(invokedMethod, semanticModel))
                    {
                        usedVariables.UnionWith(GetUsedVariablesInMethod(invokedMethod, semanticModel));
                    }
                }
            }

            return usedVariables.Count;
        }

        public static int CalculateMethodATLD(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
        {
            var usedVariables = new HashSet<ISymbol>();
            var constructorBody = constructor.Body;

            if (constructorBody != null)
            {
                var variableAccesses = constructorBody.DescendantNodes().OfType<IdentifierNameSyntax>();
                foreach (var variableAccess in variableAccesses)
                {
                    try
                    {
                        var symbol = semanticModel.GetSymbolInfo(variableAccess).Symbol;
                        if (symbol != null && IsLocalToSystem(symbol, semanticModel))
                        {
                            usedVariables.Add(symbol);
                        }
                    }
                    catch { }
                }

                var invokedMethods = constructorBody.DescendantNodes().OfType<InvocationExpressionSyntax>();
                foreach (var invocation in invokedMethods)
                {
                    var invokedMethod = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                    if (invokedMethod != null && IsLocalToSystem(invokedMethod, semanticModel))
                    {
                        usedVariables.UnionWith(GetUsedVariablesInMethod(invokedMethod, semanticModel));
                    }
                }
            }

            return usedVariables.Count;
        }

        private static HashSet<ISymbol> GetUsedVariablesInMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var usedVariables = new HashSet<ISymbol>();
            var methodBody = method.Body;

            if (methodBody != null)
            {
                var variableAccesses = methodBody.DescendantNodes().OfType<IdentifierNameSyntax>();
                foreach (var variableAccess in variableAccesses)
                {
                    try
                    {
                        var symbol = semanticModel.GetSymbolInfo(variableAccess).Symbol;
                        if (symbol != null && IsLocalToSystem(symbol, semanticModel))
                        {
                            usedVariables.Add(symbol);
                        }
                    }
                    catch { }
                }
            }

            return usedVariables;
        }

        private static HashSet<ISymbol> GetUsedVariablesInMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            var usedVariables = new HashSet<ISymbol>();

            var syntaxReference = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxReference != null)
            {
                var methodSyntax = syntaxReference.GetSyntax() as MethodDeclarationSyntax;
                if (methodSyntax != null)
                {
                    usedVariables.UnionWith(GetUsedVariablesInMethod(methodSyntax, semanticModel));
                }
            }

            return usedVariables;
        }

        private static bool IsLocalToSystem(ISymbol symbol, SemanticModel semanticModel)
        {
            return symbol.Kind == SymbolKind.Local || symbol.Kind == SymbolKind.Parameter || symbol.ContainingAssembly == semanticModel.Compilation.Assembly;
        }

        private static bool IsConstructor(MethodDeclarationSyntax method)
        {
            return method.Identifier.ValueText == method.Parent?.ChildTokens().FirstOrDefault(t => t.IsKind(SyntaxKind.IdentifierToken)).ValueText;
        }
    }
}
