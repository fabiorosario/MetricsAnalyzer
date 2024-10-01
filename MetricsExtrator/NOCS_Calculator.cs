using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    internal class NOCS_Calculator
    {
        internal static int CountNestedClasses(ClassDeclarationSyntax classDeclaration)
        {
            var nestedClasses = classDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>();
            return nestedClasses.Count();
        }
    }
}
