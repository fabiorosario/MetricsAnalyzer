﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.MethodMetrics
{
    internal class FDP_Calculator
    {
        internal int CalculateFDPForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var accessedClasses = new HashSet<INamedTypeSymbol>();

            var accessedVariables = method.DescendantNodes().OfType<IdentifierNameSyntax>();

            foreach (var variable in accessedVariables)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(variable).Symbol;

                if (symbolInfo is IFieldSymbol fieldSymbol)
                {
                    var containingClass = fieldSymbol.ContainingType;
                    if (containingClass != null && !containingClass.Equals(semanticModel.GetDeclaredSymbol(method.Parent)))
                    {
                        accessedClasses.Add(containingClass);
                    }
                }
                else if (symbolInfo is IPropertySymbol propertySymbol)
                {
                    var containingClass = propertySymbol.ContainingType;
                    if (containingClass != null && !containingClass.Equals(semanticModel.GetDeclaredSymbol(method.Parent)))
                    {
                        accessedClasses.Add(containingClass);
                    }
                }
            }

            return accessedClasses.Count;
        }

        internal int CalculateFDPForMethod(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
        {
            var accessedClasses = new HashSet<INamedTypeSymbol>();

            var accessedVariables = constructor.DescendantNodes().OfType<IdentifierNameSyntax>();

            foreach (var variable in accessedVariables)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(variable).Symbol;

                if (symbolInfo is IFieldSymbol fieldSymbol)
                {
                    var containingClass = fieldSymbol.ContainingType;
                    if (containingClass != null && !containingClass.Equals(semanticModel.GetDeclaredSymbol(constructor.Parent)))
                    {
                        accessedClasses.Add(containingClass);
                    }
                }
                else if (symbolInfo is IPropertySymbol propertySymbol)
                {
                    var containingClass = propertySymbol.ContainingType;
                    if (containingClass != null && !containingClass.Equals(semanticModel.GetDeclaredSymbol(constructor.Parent)))
                    {
                        accessedClasses.Add(containingClass);
                    }
                }
            }

            return accessedClasses.Count;
        }
    }
}
