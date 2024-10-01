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
    internal class ATFD_Calculator
    {
        internal static int CalculateATFDForClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModelCompilation)
        {
            var ATFD_type = 0;
            var className = classDeclaration.Identifier.ToString();
            var fieldDeclarations = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var fieldSymbol = semanticModelCompilation.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables.First()) as IFieldSymbol;
                if (fieldSymbol != null && fieldSymbol.ContainingType.ToString() != className)
                {
                    ATFD_type++;
                }
            }
            return ATFD_type;
        }

        internal static int CalculateATFDForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var atfdCount = 0;
            if (!method.Modifiers.Any(SyntaxKind.AbstractKeyword)
                                && method.Modifiers.Any(SyntaxKind.PublicKeyword)
                                && !IsConstructor(method))
            {

                // Find member access expressions in the method
                var memberAccessExpressions = method.DescendantNodes().OfType<MemberAccessExpressionSyntax>();

                foreach (var memberAccess in memberAccessExpressions)
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(memberAccess);
                    var symbol = symbolInfo.Symbol as IFieldSymbol;
                    if (symbol != null && !IsMemberOfSameClass(method, symbol))
                    {
                        atfdCount++;
                    }
                }
            }

            return atfdCount;
        }
        private static bool IsMemberOfSameClass(MethodDeclarationSyntax method, ISymbol symbol)
        {
            var methodClass = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            var symbolClass = symbol.ContainingType;
            return methodClass != null && symbolClass != null && methodClass.Identifier.Text == symbolClass.Name;
        }
        private static bool IsConstructor(MethodDeclarationSyntax method)
        {
            return method.Identifier.ValueText == method.Parent?.ChildTokens().FirstOrDefault(t => t.IsKind(SyntaxKind.IdentifierToken)).ValueText;
        }

    }
}
