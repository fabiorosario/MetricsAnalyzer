using MetricsExtrator.ClassMetrics;
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
    internal class MethodMetricsCalculators
    {
        internal NOLV_Calculator Nolv_Calculator { get; set; }
        internal MaMCL_Calculator Mamcl_Calculator { get; set; }
        internal MeMCL_Calculator Memcl_Calculator { get; set; }
        internal CLNAMM_Calculator Clnamm_Calculator { get; set; }
        internal ATFD_Calculator Atfd_Calculator { get; set; }
        internal CINT_Calculator Cint_Calculator { get; set; }
        internal FANOUT_Calculator Fanout_Calculator { get; set; }
        internal ATLD_Calculator Atld_Calculator { get; set; }
        internal CFNAMM_Calculator Cfnamm_Calculator { get; set; }
        internal MAXNESTING_Calculator Maxnesting_Calculator { get; set; }
        internal FDP_Calculator Fdp_Calculator { get; set; }
        internal NOAV_Calculator Noav_Calculator { get; set; }
        internal double Cdisp_method {  get; set; }

        internal MethodMetricsCalculators()
        {
            Nolv_Calculator = new NOLV_Calculator();
            Mamcl_Calculator = new MaMCL_Calculator();
            Memcl_Calculator = new MeMCL_Calculator();
            Clnamm_Calculator = new CLNAMM_Calculator();
            Atfd_Calculator = new ATFD_Calculator();
            Cint_Calculator = new CINT_Calculator();
            Fanout_Calculator = new FANOUT_Calculator();
            Atld_Calculator = new ATLD_Calculator();
            Cfnamm_Calculator = new CFNAMM_Calculator();
            Maxnesting_Calculator = new MAXNESTING_Calculator();
            Fdp_Calculator = new FDP_Calculator();
            Noav_Calculator = new NOAV_Calculator();
        }

        internal int CalculateCyclomaticComplexity(MethodDeclarationSyntax method)
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
        internal int CalculateNOPForMethod(MethodDeclarationSyntax method)
        {
            return method.ParameterList.Parameters.Count;
        }
        internal int CalculateLinesOfCode(MethodDeclarationSyntax method)
        {
            return method.GetLocation().GetLineSpan().EndLinePosition.Line -
                   method.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        }
    }
}
