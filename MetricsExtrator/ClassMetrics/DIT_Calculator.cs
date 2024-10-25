using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.ClassMetrics
{
    internal class DIT_Calculator
    {
        internal int CalculateClassDIT(INamedTypeSymbol classSymbol, Project project)
        {
            int depth = 0;
            var currentClass = classSymbol != null ? classSymbol.BaseType : null;

            while (currentClass != null && currentClass.SpecialType != SpecialType.System_Object)
            {
                // Check if the currentClass belongs to the current project (system)
                var sourceTree = currentClass.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;
                if (sourceTree != null && project.Documents.Any(doc => doc.FilePath == sourceTree.FilePath))
                {
                    depth++;
                }
                else
                {
                    // Stop counting if the class does not belong to the system
                    break;
                }

                currentClass = currentClass.BaseType;
            }

            /*Definition: the depth of a class, measured by DIT, within the inheritance hierarchy is the maximum 
            length from the class node to the root of the tree, measured by the number of ancestor classes.DIT has
            a minimum value of one, for classes that do not have ancestors*/
            if (depth == 0)
                depth = 1;

            return depth;
        }
    }
}
