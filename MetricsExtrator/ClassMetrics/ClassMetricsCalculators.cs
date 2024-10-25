using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsExtrator.ClassMetrics
{
    internal class ClassMetricsCalculators
    {
        internal LOCNAMM_Calculator Locnamm_Calculator { get; set; }
        internal DIT_Calculator Dit_Calculator { get; set; }
        internal RFC_Calculator Rfc_Calculator { get; set; }
        internal LCOM5_Calculator Lcom5_Calculator { get; set; }
        internal TCC_Calculator Tcc_Calculator { get; set; }
        internal double Wmc_type { get; set; }
        internal double Wmcnamm_type { get; set; }
        internal double Amw_type { get; set; }
        internal double Amwnamm_type { get; set; }
        internal double Nomnamm_type { get; set; }
        internal double Fanout_type { get; set; }
        internal int Cfnamm_type { get; set; }
        internal int Noam_type { get; set; }
        internal int Atfd_type { get; set; }

        internal ClassMetricsCalculators()
        {
            Locnamm_Calculator = new LOCNAMM_Calculator();
            Dit_Calculator = new DIT_Calculator();
            Rfc_Calculator = new RFC_Calculator();
            Lcom5_Calculator = new LCOM5_Calculator();
            Tcc_Calculator = new TCC_Calculator();
        }

       
        internal int CalculateNumberOfInheritedMethods(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Count(method => !method.Modifiers.Any(SyntaxKind.StaticKeyword));
        }
        internal double CalculateWeightOfClass(ClassDeclarationSyntax classDeclaration)
        {
            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
            var totalMethods = methods.Count();
            var publicMethods = methods.Count(m => m.Modifiers.Any(SyntaxKind.PublicKeyword) && !m.Modifiers.Any(SyntaxKind.AbstractKeyword));

            return totalMethods > 0 ? (double)publicMethods / totalMethods : 0;
        }
        internal int CalculateLinesOfCode(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.GetLocation().GetLineSpan().EndLinePosition.Line -
                   classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        }
        internal int CountNestedClasses(ClassDeclarationSyntax classDeclaration)
        {
            var nestedClasses = classDeclaration.DescendantNodes().OfType<ClassDeclarationSyntax>();
            return nestedClasses.Count();
        }
        internal int CountMethodsClass(ClassDeclarationSyntax classDeclaration)
        {
            var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
            return methods.Count();
        }
        internal int CalculateClassNOA(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Members.OfType<FieldDeclarationSyntax>().Count();
        }
        internal int CalculateNonFinalNonStaticAttributes(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var fields = classDeclaration.Members.OfType<FieldDeclarationSyntax>();
            int nonFinalNonStaticAttributesCount = 0;

            foreach (var field in fields)
            {
                bool isStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword);
                bool isFinal = field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword);

                if (!isStatic && !isFinal)
                {
                    nonFinalNonStaticAttributesCount += field.Declaration.Variables.Count;
                }
            }

            return nonFinalNonStaticAttributesCount;
        }
        internal int CalculateNumberOfPrivateAttributes(ClassDeclarationSyntax classDeclaration)
        {
            // Obter todos os membros do tipo FieldDeclaration (atributos)
            var privateAttributes = classDeclaration.Members
                .OfType<FieldDeclarationSyntax>()
                .Where(field => field.Modifiers.Any(SyntaxKind.PrivateKeyword));

            // Retornar a contagem de atributos privados
            return privateAttributes.Count();
        }
        internal int CalculateATFDForClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModelCompilation)
        {
            var ATFD_type = 0;
            var className = classDeclaration.Identifier.ToString();
            var fieldDeclarations = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var fieldSymbol = semanticModelCompilation.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables.First()) as IFieldSymbol;
                if (fieldSymbol != null && fieldSymbol.ContainingType.ToString() != className)
                {
                    ATFD_type++;
                }
            }
            return ATFD_type;
        }
    }
}
