using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.ClassMetrics
{
    internal class LOCNAMM_Calculator
    {
        internal int CalculateLOCNAMMForClass(ClassDeclarationSyntax classDeclaration)
        {
            var totalLines = 0;
            var methodLines = 0;
            var propertyLines = 0;

            try
            {
                totalLines = classDeclaration.GetLocation().GetLineSpan().EndLinePosition.Line -
                         classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            }
            catch { }

            try
            {
                methodLines = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                              .Sum(m => m.GetLocation().GetLineSpan().EndLinePosition.Line -
                                                        m.GetLocation().GetLineSpan().StartLinePosition.Line + 1);
            }
            catch { }

            try
            {
                propertyLines = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>()
                                                .Sum(p => p.AccessorList.Accessors
                                                          .Sum(a => a.GetLocation().GetLineSpan().EndLinePosition.Line -
                                                                    a.GetLocation().GetLineSpan().StartLinePosition.Line + 1));
            }
            catch { }

            var mainMethod = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                             .FirstOrDefault(m => m.Identifier.Text == "Main");

            var mainMethodLines = mainMethod != null
                ? mainMethod.GetLocation().GetLineSpan().EndLinePosition.Line - mainMethod.GetLocation().GetLineSpan().StartLinePosition.Line + 1
                : 0;

            var locnamm = totalLines - methodLines - propertyLines - mainMethodLines;

            if (locnamm < 0)
                locnamm = 0;

            return locnamm;
        }
    }
}
