using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    internal class MetricsUtilities
    {
        internal bool IsAccessorOrMutator(MethodDeclarationSyntax method)
        {
            // Identify accessors (getters and setters)
            var parent = method.Parent as PropertyDeclarationSyntax;
            if (parent != null)
            {
                var accessorList = parent.AccessorList?.Accessors;
                if (accessorList != null)
                {
                    foreach (var accessor in accessorList)
                    {
                        if (accessor.Keyword.IsKind(SyntaxKind.GetKeyword) || accessor.Keyword.IsKind(SyntaxKind.SetKeyword))
                        {
                            return true;
                        }
                    }
                }
            }

            // Additional checks for mutators (like add and remove in event handlers) can be added here if needed
            return false;
        } 
        internal bool IsAccessesMember(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var methodSymbol = semanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
            return method.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Select(id => semanticModel.GetSymbolInfo(id).Symbol)
                .OfType<IFieldSymbol>()
                .Any(field => field.ContainingType == methodSymbol.ContainingType);
        }
        internal string GetNamespace(ClassDeclarationSyntax classDeclaration)
        {
            var namespaceDeclaration = classDeclaration.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            return namespaceDeclaration != null ? namespaceDeclaration.Name.ToString() : string.Empty;
        }
        internal bool IsConstructor(MethodDeclarationSyntax method)
        {
            return method.Identifier.ValueText == method.Parent?.ChildTokens().FirstOrDefault(t => t.IsKind(SyntaxKind.IdentifierToken)).ValueText;
        }
        internal bool IsVariableSymbol(ISymbol symbol)
        {
            return symbol.Kind == SymbolKind.Local ||
                   symbol.Kind == SymbolKind.Parameter ||
                   symbol.Kind == SymbolKind.Field ||
                   symbol.Kind == SymbolKind.Property;
        }
    }
}
