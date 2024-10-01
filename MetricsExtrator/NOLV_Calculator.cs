using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator
{
    internal class NOLV_Calculator
    {
        internal static int CalculateNOLVForMethod(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            var variables = new HashSet<ISymbol>();

            foreach (var descendant in method.DescendantNodes())
            {
                try
                {
                    if (descendant is VariableDeclaratorSyntax variableDeclarator)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(variableDeclarator);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is ParameterSyntax parameter)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(parameter);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is CatchClauseSyntax catchClause)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(catchClause.Declaration.Identifier.Parent);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is EnumMemberDeclarationSyntax enumMember)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(enumMember);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is VariableDeclarationSyntax variableDeclaration)
                    {
                        var symbolInfos = variableDeclaration.Variables.Select(v => semanticModel.GetDeclaredSymbol(v));
                        foreach (var symbolInfo in symbolInfos)
                        {
                            if (symbolInfo != null && IsVariableSymbol(symbolInfo))
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

        internal static int CalculateNOLVForMethod(ConstructorDeclarationSyntax constructor, SemanticModel semanticModel)
        {
            var variables = new HashSet<ISymbol>();

            foreach (var descendant in constructor.DescendantNodes())
            {
                try
                {
                    if (descendant is VariableDeclaratorSyntax variableDeclarator)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(variableDeclarator);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is ParameterSyntax parameter)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(parameter);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is CatchClauseSyntax catchClause)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(catchClause.Declaration.Identifier.Parent);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is EnumMemberDeclarationSyntax enumMember)
                    {
                        var symbolInfo = semanticModel.GetDeclaredSymbol(enumMember);
                        if (symbolInfo != null && IsVariableSymbol(symbolInfo))
                        {
                            variables.Add(symbolInfo);
                        }
                    }
                    else if (descendant is VariableDeclarationSyntax variableDeclaration)
                    {
                        var symbolInfos = variableDeclaration.Variables.Select(v => semanticModel.GetDeclaredSymbol(v));
                        foreach (var symbolInfo in symbolInfos)
                        {
                            if (symbolInfo != null && IsVariableSymbol(symbolInfo))
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

        private static bool IsVariableSymbol(ISymbol symbol)
        {
            return symbol.Kind == SymbolKind.Local ||
                   symbol.Kind == SymbolKind.Parameter ||
                   symbol.Kind == SymbolKind.Field ||
                   symbol.Kind == SymbolKind.Property;
        }
    }
}
