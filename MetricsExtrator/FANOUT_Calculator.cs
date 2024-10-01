using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    internal class FANOUT_Calculator
    {
        internal static int CalculateFANOUTForMethod(IEnumerable<IMethodSymbol> invokedMethods)
        {
            var uniqueCalledClasses = new HashSet<INamedTypeSymbol>();
            foreach (var invokedMethod in invokedMethods)
            {
                uniqueCalledClasses.Add(invokedMethod.ContainingType);
            }

            return uniqueCalledClasses.Count();
        }
    }
}
