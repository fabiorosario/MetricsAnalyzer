using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.MethodMetrics
{
    internal class ATFD_Calculator
    {
        MetricsUtilities metricsUtilities;
        public ATFD_Calculator()
        {
            metricsUtilities = new MetricsUtilities();
        }
        internal int CalculateATFDForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var atfdCount = 0;
            if (!method.Modifiers.Any(SyntaxKind.AbstractKeyword)
                                && method.Modifiers.Any(SyntaxKind.PublicKeyword)
                                && !metricsUtilities.IsConstructor(method))
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
        private bool IsMemberOfSameClass(MethodDeclarationSyntax method, ISymbol symbol)
        {
            var methodClass = method.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            var symbolClass = symbol.ContainingType;
            return methodClass != null && symbolClass != null && methodClass.Identifier.Text == symbolClass.Name;
        }
    }
}
