using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.MethodMetrics
{
    internal class CLNAMM_Calculator
    {
        MetricsUtilities metricsUtilities;
        public CLNAMM_Calculator()
        {
            metricsUtilities = new MetricsUtilities();
        }
        internal int CalculateCLNAMMForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            // Obter todos os métodos na classe atual que não são acessores ou mutadores
            var classDeclaration = method.Parent as ClassDeclarationSyntax;
            if (classDeclaration == null)
                return 0;

            var localMethods = classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Where(m => !metricsUtilities.IsAccessorOrMutator(m));

            var clnammCount = 0;

            // Iterar sobre todas as chamadas de método no método medido
            var invocationExpressions = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var invocation in invocationExpressions)
            {
                var symbolInfo = semanticModel.GetSymbolInfo(invocation);
                var methodSymbol = symbolInfo.Symbol as IMethodSymbol;

                // Verificar se o método chamado é um método local e não é acessor ou mutador
                if (methodSymbol != null && localMethods.Any(m => semanticModel.GetDeclaredSymbol(m).Equals(methodSymbol)))
                {
                    clnammCount++;
                }
            }

            return clnammCount;
        }
    }
}
