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
    internal class MethodMetricsUtilities
    {
        internal static bool IsAccessorOrMutator(MethodDeclarationSyntax method)
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
        internal static int CalculateCyclomaticComplexity(MethodDeclarationSyntax method)
        {
            var complexity = 1; // Base complexity

            // Add complexity for each decision point
            var descendantNodes = method.DescendantNodes();
            complexity += descendantNodes.Count(node => node is IfStatementSyntax);
            complexity += descendantNodes.Count(node => node is ForStatementSyntax);
            complexity += descendantNodes.Count(node => node is ForEachStatementSyntax);
            complexity += descendantNodes.Count(node => node is WhileStatementSyntax);
            complexity += descendantNodes.Count(node => node is SwitchStatementSyntax);
            complexity += descendantNodes.Count(node => node is DoStatementSyntax);
            complexity += descendantNodes.Count(node => node is CaseSwitchLabelSyntax);
            complexity += descendantNodes.Count(node => node is ConditionalExpressionSyntax);
            complexity += descendantNodes.Count(node => node is CatchClauseSyntax);
            complexity += descendantNodes.Count(node => node is BinaryExpressionSyntax binaryExpr && binaryExpr.IsKind(SyntaxKind.LogicalAndExpression));
            complexity += descendantNodes.Count(node => node is BinaryExpressionSyntax binaryExpr && binaryExpr.IsKind(SyntaxKind.LogicalOrExpression));

            return complexity;
        }
        internal static int CalculateLinesOfCode(MethodDeclarationSyntax method)
        {
            return method.GetLocation().GetLineSpan().EndLinePosition.Line -
                   method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        }
        internal static bool IsAccessesMember(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var methodSymbol = semanticModel.GetDeclaredSymbol(method) as IMethodSymbol;
            return method.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Select(id => semanticModel.GetSymbolInfo(id).Symbol)
                .OfType<IFieldSymbol>()
                .Any(field => field.ContainingType == methodSymbol.ContainingType);
        }

        internal static int CalculateNonFinalNonStaticAttributes(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var fields = classDeclaration.Members.OfType<FieldDeclarationSyntax>();
            int nonFinalNonStaticAttributesCount = 0;

            foreach (var field in fields)
            {
                bool isStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword);
                bool isFinal = field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword);

                if (!isStatic && !isFinal)
                {
                    nonFinalNonStaticAttributesCount += field.Declaration.Variables.Count;
                }
            }

            return nonFinalNonStaticAttributesCount;
        }

        internal static int CalculateNumberOfPrivateAttributes(ClassDeclarationSyntax classDeclaration)
        {
            // Obter todos os membros do tipo FieldDeclaration (atributos)
            var privateAttributes = classDeclaration.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(field => field.Modifiers.Any(SyntaxKind.PrivateKeyword));

            // Retornar a contagem de atributos privados
            return privateAttributes.Count();
        }

    }
}
