using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MetricsExtrator
{
    internal class ClassMetricsUtilities
    {
        internal static int CalculateNumberOfInheritedMethods(ClassDeclarationSyntax classDeclaration)
        {     
            return classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Count(method => !method.Modifiers.Any(SyntaxKind.StaticKeyword));
        }
        internal static double CalculateWeightOfClass(ClassDeclarationSyntax classDeclaration)
        {
            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
            var totalMethods = methods.Count();
            var publicMethods = methods.Count(m => m.Modifiers.Any(SyntaxKind.PublicKeyword) && !m.Modifiers.Any(SyntaxKind.AbstractKeyword));

            return totalMethods > 0 ? (double)publicMethods / totalMethods : 0;
        }
        internal static int CalculateLinesOfCode(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.GetLocation().GetLineSpan().EndLinePosition.Line -
                   classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        }

    }
}
