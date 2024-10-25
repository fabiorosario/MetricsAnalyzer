using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.MethodMetrics
{
    internal class MaMCL_Calculator
    {
        internal int CalculateMaMCLForMethod(MethodDeclarationSyntax method)
        {
            int maxChainLength = 0;

            // Verifica todas as chamadas de método dentro do método
            var invocations = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var invocation in invocations)
            {
                int chainLength = CalculateChainLength(invocation);
                if (chainLength > maxChainLength)
                {
                    maxChainLength = chainLength;
                }
            }

            return maxChainLength;
        }

        private int CalculateChainLength(InvocationExpressionSyntax invocation)
        {
            int length = 1; // Uma chamada já tem comprimento 1
            var current = invocation.Expression;

            while (current is MemberAccessExpressionSyntax memberAccess)
            {
                length++;
                current = memberAccess.Expression;
            }

            // O comprimento mínimo de uma cadeia é 2
            return length >= 2 ? length : 0;
        }
    }
}
