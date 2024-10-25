using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.MethodMetrics
{
    internal class NOLV_Calculator
    {
        MetricsUtilities metricsUtilities;

        public NOLV_Calculator()
        {
            metricsUtilities = new MetricsUtilities();
        }
        public int CalculateNOLVForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var variables = new HashSet<ISymbol>();

            foreach (var descendant in method.DescendantNodes())
            {
                try
                {
                    if (descendant is VariableDeclaratorSyntax variableDeclarator)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(variableDeclarator);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is ParameterSyntax parameter)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(parameter);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is CatchClauseSyntax catchClause)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(catchClause.Declaration.Identifier.Parent);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is EnumMemberDeclarationSyntax enumMember)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(enumMember);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is VariableDeclarationSyntax variableDeclaration)
                    {
                        var symbolInfos = variableDeclaration.Variables.Select(v => semanticModel.GetDeclaredSymbol(v));
                        foreach (var symbolInfo in symbolInfos)
                        {
                            if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                            {
                                variables.Add(symbolInfo);
                            }
                        }
                    }
                }
                catch { }
            }

            return variables.Count;
        }

        public int CalculateNOLVForMethod(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
        {
            var variables = new HashSet<ISymbol>();

            foreach (var descendant in constructor.DescendantNodes())
            {
                try
                {
                    if (descendant is VariableDeclaratorSyntax variableDeclarator)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(variableDeclarator);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is ParameterSyntax parameter)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(parameter);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is CatchClauseSyntax catchClause)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(catchClause.Declaration.Identifier.Parent);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is EnumMemberDeclarationSyntax enumMember)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(enumMember);
                        if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is VariableDeclarationSyntax variableDeclaration)
                    {
                        var symbolInfos = variableDeclaration.Variables.Select(v => semanticModel.GetDeclaredSymbol(v));
                        foreach (var symbolInfo in symbolInfos)
                        {
                            if (symbolInfo != null && metricsUtilities.IsVariableSymbol(symbolInfo))
                            {
                                variables.Add(symbolInfo);
                            }
                        }
                    }
                }
                catch { }
            }

            return variables.Count;
        }
    }
}
