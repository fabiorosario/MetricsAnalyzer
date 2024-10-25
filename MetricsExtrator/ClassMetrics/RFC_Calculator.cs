using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsExtrator.ClassMetrics
{
    internal class RFC_Calculator
    {
        internal int CalculateRFCForClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var responseSet = new HashSet<IMethodSymbol>();

            var methods = classDeclaration.Members.OfType<MethodDeclarationSyntax>();

            foreach (var method in methods)
            {
                AddMethodAndCalledMethodsToResponseSet(method, semanticModel, responseSet);
            }

            var baseType = semanticModel.GetDeclaredSymbol(classDeclaration)?.BaseType;
            while (baseType != null && baseType.ContainingNamespace.Name != "System")
            {
                var baseTypeMethods = baseType.GetMembers().OfType<IMethodSymbol>();
                foreach (var method in baseTypeMethods)
                {
                    responseSet.Add(method);
                    AddCalledMethodsToResponseSet(method, responseSet, semanticModel);
                }
                baseType = baseType.BaseType;
            }

            return responseSet.Count;
        }

        private void AddMethodAndCalledMethodsToResponseSet(MethodDeclarationSyntax method, SemanticModel semanticModel, HashSet<IMethodSymbol> responseSet)
        {
            var methodSymbol = semanticModel.GetDeclaredSymbol(method);
            if (methodSymbol != null)
            {
                responseSet.Add(methodSymbol);
                AddCalledMethodsToResponseSet(method, responseSet, semanticModel);
            }
        }

        private void AddCalledMethodsToResponseSet(SyntaxNode methodNode, HashSet<IMethodSymbol> responseSet, SemanticModel semanticModel)
        {
            var calledMethods = methodNode.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var calledMethod in calledMethods)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(calledMethod.Expression).Symbol as IMethodSymbol;
                if (symbolInfo != null && !responseSet.Contains(symbolInfo))
                {
                    responseSet.Add(symbolInfo);
                }
            }
        }

        private void AddCalledMethodsToResponseSet(IMethodSymbol method, HashSet<IMethodSymbol> responseSet, SemanticModel semanticModel)
        {
            try
            {
                var syntaxReference = method.DeclaringSyntaxReferences.FirstOrDefault();
                if (syntaxReference != null)
                {
                    var methodNode = syntaxReference.GetSyntax();
                    var calledMethods = methodNode.DescendantNodes().OfType<InvocationExpressionSyntax>();
                    foreach (var calledMethod in calledMethods)
                    {
                        var symbolInfo = semanticModel.GetSymbolInfo(calledMethod.Expression).Symbol as IMethodSymbol;
                        if (symbolInfo != null && !responseSet.Contains(symbolInfo))
                        {
                            responseSet.Add(symbolInfo);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
