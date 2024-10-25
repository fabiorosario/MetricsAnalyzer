using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.MethodMetrics
{
    internal class NOAV_Calculator
    {
        MetricsUtilities metricsUtilities;

        public NOAV_Calculator()
        {
            metricsUtilities = new MetricsUtilities();
        }

        internal int CalculateNOAVForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
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

            // Collect variables accessed through accessor methods
            var calledMethods = method.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var calledMethod in calledMethods)
            {
                try
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(calledMethod).Symbol as IMethodSymbol;
                    if (symbolInfo != null)
                    {
                        foreach (var accessedVariable in GetVariablesAccessedByMethod(symbolInfo, semanticModel))
                        {
                            variables.Add(accessedVariable);
                        }
                    }
                }
                catch { }
            }

            return variables.Count;
        }

        internal int CalculateNOAVForMethod(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
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

            // Collect variables accessed through accessor methods
            var calledMethods = constructor.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var calledMethod in calledMethods)
            {
                try
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(calledMethod).Symbol as IMethodSymbol;
                    if (symbolInfo != null)
                    {
                        foreach (var accessedVariable in GetVariablesAccessedByMethod(symbolInfo, semanticModel))
                        {
                            variables.Add(accessedVariable);
                        }
                    }
                }
                catch { }
            }

            return variables.Count;
        }

        private IEnumerable<ISymbol> GetVariablesAccessedByMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            var variables = new HashSet<ISymbol>();
            var methodDeclaration = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;
            if (methodDeclaration != null)
            {
                foreach (var descendant in methodDeclaration.DescendantNodes())
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
            }
            return variables;
        }
    }
}
