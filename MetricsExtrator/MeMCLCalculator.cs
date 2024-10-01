using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    internal class MeMCLCalculator
    {
        internal static double CalculateMeMCLForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var invocationExpressions = method.DescendantNodes().OfType<InvocationExpressionSyntax>();

            if (!invocationExpressions.Any())
            {
                return 0;
            }

            double totalChainLength = 0;
            int chainCount = 0;

            foreach (var invocation in invocationExpressions)
            {
                int chainLength = CalculateChainLength(invocation, semanticModel);
                if (chainLength >= 2)
                {
                    totalChainLength += chainLength;
                    chainCount++;
                }
            }

            return chainCount > 0 ? totalChainLength / chainCount : 0;
        }

        private static int CalculateChainLength(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            int length = 1;
            var currentNode = invocation.Expression;

            while (currentNode is MemberAccessExpressionSyntax memberAccess)
            {
                length++;
                currentNode = memberAccess.Expression;
            }

            return length;
        }
    }
}
