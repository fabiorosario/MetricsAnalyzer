using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.MethodMetrics
{
    internal class MAXNESTING_Calculator
    {
        internal int GetMaxNestingLevel(BlockSyntax body)
        {
            if (body == null) return 0;

            var maxNesting = 0;
            var currentNesting = 0;

            void CalculateNesting(SyntaxNode node)
            {
                if (node is StatementSyntax statement && IsControlFlowStatement(statement))
                {
                    currentNesting++;
                    if (currentNesting > maxNesting)
                    {
                        maxNesting = currentNesting;
                    }
                }

                foreach (var child in node.ChildNodes())
                {
                    CalculateNesting(child);
                }

                if (node is StatementSyntax && IsControlFlowStatement(node))
                {
                    currentNesting--;
                }
            }

            CalculateNesting(body);
            return maxNesting;
        }

        private bool IsControlFlowStatement(SyntaxNode node)
        {
            return node is IfStatementSyntax ||
                   node is ForStatementSyntax ||
                   node is WhileStatementSyntax ||
                   node is DoStatementSyntax ||
                   node is SwitchStatementSyntax ||
                   node is TryStatementSyntax ||
                   node is UsingStatementSyntax ||
                   node is ForEachStatementSyntax ||
                   node is LockStatementSyntax;
        }
    }
}
