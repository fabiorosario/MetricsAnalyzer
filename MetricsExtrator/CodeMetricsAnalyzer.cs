using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using Microsoft.Build.Evaluation;

namespace MetricsExtrator
{
    class CodeMetricsAnalyzer
    {
        private const string LC = "Large Class";
        private const string DC = "Data Class";
        private const string RB = "Refused Bequest";
        private const string LM = "Long Method";
        private const string FE = "Feature Envy";
        private static bool isMethodCodeSmell = true;
        private static int totalFiles;
        private static int processedFiles;
        private static Dictionary<string, double> metricsResults;
        private static string currentNamespace = string.Empty;
        private static string currentProject = string.Empty;
        private static string currentKeyBase = string.Empty;
        private static string currentKey = string.Empty;

        public static async Task<List<LabeledCodeSmellMetrics>> CalculateMetrics(string projectPath, string codeSmell, IProgress<double> progress)
        {
            
            List<LabeledCodeSmellMetrics> labeledCodeSmellMetricsList = new List<LabeledCodeSmellMetrics>();

            var labeledCodeSmell = LabeledCodeSmell(codeSmell);

            //var workspace = MSBuildWorkspace.Create();
            //var solution = await workspace.OpenSolutionAsync(projectPath);

            //foreach (var project in solution.Projects)
            //{

            
            
            metricsResults = new Dictionary<string, double>();

            var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectPath);

            processedFiles = 0;
            totalFiles = project.Documents.Count();

            currentNamespace = string.Empty;
            currentProject = project.Name;
            currentKeyBase = string.Empty;
            currentKey = string.Empty;
            var compilation = await project.GetCompilationAsync();

            foreach (var document in project.Documents)
            {
                var root = await document.GetSyntaxRootAsync();
                if (root == null) continue;

                var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
                var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                var semanticModelCompilation = compilation.GetSemanticModel(root.SyntaxTree);
                var semanticModelDocument = await document.GetSemanticModelAsync();

                foreach (var classDeclaration in classDeclarations)
                {
                    currentNamespace = GetNamespace(classDeclaration);

                    var className = classDeclaration.Identifier.ToString();

                    var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
                    var classSymbol = semanticModelDocument.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

                    var LOC_type = 0;//ClassMetricsUtilities.CalculateLinesOfCode(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.LOC_type";
                    AddMetricResult(metricsResults, currentKey, LOC_type);

                    var LOC_package = LOC_type;
                    currentKey = $"{currentProject}.{currentNamespace}.LOC_package";
                    AddMetricResult(metricsResults, currentKey, LOC_package);

                    var LOC_project = LOC_type;
                    currentKey = $"{currentProject}.LOC_project";
                    AddMetricResult(metricsResults, currentKey, LOC_project);

                    var LOCNAMM_type = 0;//LOCNAMM_Calculator.CalculateLOCNAMMForClass(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.LOCNAMM_type";
                    AddMetricResult(metricsResults, currentKey, LOCNAMM_type);

                    var LOCNAMM_package = LOCNAMM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.LOCNAMM_package";
                    AddMetricResult(metricsResults, currentKey, LOCNAMM_package);

                    var LOCNAMM_project = LOCNAMM_type;
                    currentKey = $"{currentProject}.LOCNAMM_project";
                    AddMetricResult(metricsResults, currentKey, LOCNAMM_project);

                    var NOCS_type = 0;//NOCS_Calculator.CountNestedClasses(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOCS_type";
                    AddMetricResult(metricsResults, currentKey, NOCS_type);

                    var NOCS_package = NOCS_type + 1; // Include the class itself
                    currentKey = $"{currentProject}.{currentNamespace}.NOCS_package";
                    AddMetricResult(metricsResults, currentKey, NOCS_package);

                    var NOCS_project = NOCS_type + 1; // Include the class itself
                    currentKey = $"{currentProject}.NOCS_project";
                    AddMetricResult(metricsResults, currentKey, NOCS_project);

                    double NOM_type = methods.Count();
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOM_type";
                    AddMetricResult(metricsResults, currentKey, NOM_type);

                    double NOM_package = NOM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.NOM_package";
                    AddMetricResult(metricsResults, currentKey, NOM_package);

                    double NOM_project = NOM_type;
                    currentKey = $"{currentProject}.NOM_project";
                    AddMetricResult(metricsResults, currentKey, NOM_project);

                    var DIT_type = 0;//DIT_Calculator.CalculateClassDIT(classSymbol, project);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.DIT_type";
                    AddMetricResult(metricsResults, currentKey, DIT_type);

                    var NOA_type = classDeclaration.Members.OfType<FieldDeclarationSyntax>().Count();
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOA_type";
                    AddMetricResult(metricsResults, currentKey, NOA_type);

                    var NIM_type = 0;//ClassMetricsUtilities.CalculateNumberOfInheritedMethods(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NIM_type";
                    AddMetricResult(metricsResults, currentKey, NIM_type);

                    var WOC_type = 0;//ClassMetricsUtilities.CalculateWeightOfClass(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.WOC_type";
                    AddMetricResult(metricsResults, currentKey, WOC_type);

                    var RFC_type = 0;// RFC_Calculator.CalculateRFCForClass(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.RFC_type";
                    AddMetricResult(metricsResults, currentKey, RFC_type);

                    var LCOM5_type = 0;//LCOM5_Calculator.CalculateLCOM5ForClass(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.LCOM5_type";
                    AddMetricResult(metricsResults, currentKey, LCOM5_type);

                    var TCC_type = 0;//TCC_Calculator.CalculateTCCForClass(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.TCC_type";
                    AddMetricResult(metricsResults, currentKey, TCC_type);

                    var num_not_final_not_static_attributes = MethodMetricsUtilities.CalculateNonFinalNonStaticAttributes(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.num_not_final_not_static_attributes";
                    AddMetricResult(metricsResults, currentKey, num_not_final_not_static_attributes);

                    var number_private_visibility_attributes = MethodMetricsUtilities.CalculateNumberOfPrivateAttributes(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.number_private_visibility_attributes";
                    AddMetricResult(metricsResults, currentKey, number_private_visibility_attributes);

                    //Method Metrics for Class Code Smell

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.CYCLO_method";
                    AddMetricResult(metricsResults, currentKey, 1);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.LOC_method";
                    AddMetricResult(metricsResults, currentKey, 3);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.ATFD_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.CINT_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.FANOUT_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.CDISP_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.ATLD_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.CFNAMM_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.MAXNESTING_method";
                    AddMetricResult(metricsResults, currentKey, 1);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.FDP_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOAV_method";
                    AddMetricResult(metricsResults, currentKey, 1);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOLV_method";
                    AddMetricResult(metricsResults, currentKey, 1);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOP_method";
                    AddMetricResult(metricsResults, currentKey, 1);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.MaMCL_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.MeMCL_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.CLNAMM_method";
                    AddMetricResult(metricsResults, currentKey, 0);

                    //class metrics that will be calculated during or after the foreach interaction statement of the class methods.
                    double WMC_type = 0;
                    double WMCNAMM_type = 0;
                    double AMW_type = 0;
                    double AMWNAMM_type = 0;
                    double NOMNAMM_type = 0;
                    double FANOUT_type = 0;
                    var CFNAMM_type = 0;
                    var NOAM_type = 0;
                    var ATFD_type = 0;//ATFD_Calculator.CalculateATFDForClass(classDeclaration, semanticModelCompilation);

                    foreach (var method in methods)
                    {
                        var nameMethod = $"{method.Identifier}({string.Join(", ", method.ParameterList.Parameters)})";

                        double CYCLO_method = 0;// MethodMetricsUtilities.CalculateCyclomaticComplexity(method);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CYCLO_method";
                        AddMetricResult(metricsResults, currentKey, CYCLO_method);
                        WMC_type += CYCLO_method;

                        if (MethodMetricsUtilities.IsAccessorOrMutator(method))
                        {
                            NOAM_type++;
                        }
                        else
                        {
                            WMCNAMM_type += CYCLO_method;
                        }

                        var NOP_method = method.ParameterList.Parameters.Count;
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.NOP_method";
                        AddMetricResult(metricsResults, currentKey, NOP_method);

                        var MaMCL_method = MaMCLCalculator.CalculateMaMCLForMethod(method);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.MaMCL_method";
                        AddMetricResult(metricsResults, currentKey, MaMCL_method);

                        var MeMCL_method = MeMCLCalculator.CalculateMeMCLForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.MeMCL_method";
                        AddMetricResult(metricsResults, currentKey, MeMCL_method);

                        var CLNAMM_method = CLNAMMCalculator.CalculateCLNAMMForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CLNAMM_method";
                        AddMetricResult(metricsResults, currentKey, CLNAMM_method);

                        var LOC_method = 0;// MethodMetricsUtilities.CalculateLinesOfCode(method);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.LOC_method";
                        AddMetricResult(metricsResults, currentKey, LOC_method);

                        var ATFD_method = 0;// ATFD_Calculator.CalculateATFDForMethod(method, semanticModelCompilation);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.ATFD_method";
                        AddMetricResult(metricsResults, currentKey, ATFD_method);
                        ATFD_type += ATFD_method;

                        var accessesMember = MethodMetricsUtilities.IsAccessesMember(method, semanticModelDocument);
                        if (!accessesMember)
                        {
                            NOMNAMM_type++;
                        }

                        var invokedMethods = 0;// CINT_Calculator.CalculateCINTForMethod(method, semanticModelDocument, classDeclaration);

                        double CINT_method = 0;// invokedMethods.Count();
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CINT_method";
                        AddMetricResult(metricsResults, currentKey, CINT_method);

                        double FANOUT_method = 0;// FANOUT_Calculator.CalculateFANOUTForMethod(invokedMethods);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.FANOUT_method";
                        AddMetricResult(metricsResults, currentKey, FANOUT_method);
                        FANOUT_type += FANOUT_method;

                        double CDISP_method = 0;
                        if (CINT_method != 0)
                            CDISP_method = FANOUT_method / CINT_method;
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CDISP_method";
                        AddMetricResult(metricsResults, currentKey, CDISP_method);

                        var ATLD_method = 0;// ATLD_Calculator.CalculateMethodATLD(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.ATLD_method";
                        AddMetricResult(metricsResults, currentKey, ATLD_method);

                        var CFNAMM_method = 0;// CFNAMM_Calculator.CalculateMethodCFNAMM(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CFNAMM_method";
                        AddMetricResult(metricsResults, currentKey, CFNAMM_method);

                        if (!IsConstructor(method) && !method.Modifiers.Any(SyntaxKind.AbstractKeyword))
                        {
                            CFNAMM_type += CFNAMM_method;
                        }

                        var MAXNESTING_method = 0;// MAXNESTING_Calculator.GetMaxNestingLevel(method.Body);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.MAXNESTING_method";
                        AddMetricResult(metricsResults, currentKey, MAXNESTING_method);

                        var FDP_method = 0;// FDP_Calculator.CalculateFDPForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.FDP_method";
                        AddMetricResult(metricsResults, currentKey, FDP_method);

                        var NOAV_method = 0;// NOAV_Calculator.CalculateNOAVForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.NOAV_method";
                        AddMetricResult(metricsResults, currentKey, NOAV_method);

                        var NOLV_method = 0;// NOLV_Calculator.CalculateNOLVForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.NOLV_method";
                        AddMetricResult(metricsResults, currentKey, NOLV_method);
                    }

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.FANOUT_type";
                    AddMetricResult(metricsResults, currentKey, FANOUT_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.CFNAMM_type";
                    AddMetricResult(metricsResults, currentKey, CFNAMM_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOAM_type";
                    AddMetricResult(metricsResults, currentKey, NOAM_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.WMCNAMM_type";
                    AddMetricResult(metricsResults, currentKey, WMCNAMM_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.WMC_type";
                    AddMetricResult(metricsResults, currentKey, WMC_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.ATFD_type";
                    AddMetricResult(metricsResults, currentKey, ATFD_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOMNAMM_type";
                    AddMetricResult(metricsResults, currentKey, NOMNAMM_type);

                    double NOMNAMM_package = NOMNAMM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.NOMNAMM_package";
                    AddMetricResult(metricsResults, currentKey, NOMNAMM_package);

                    double NOMNAMM_project = NOMNAMM_type;
                    currentKey = $"{currentProject}.NOMNAMM_project";
                    AddMetricResult(metricsResults, currentKey, NOMNAMM_project);

                    if (NOMNAMM_type != 0)
                        AMWNAMM_type = WMCNAMM_type / NOMNAMM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.AMWNAMM_type";
                    AddMetricResult(metricsResults, currentKey, AMWNAMM_type);


                    if (NOM_type != 0)
                        AMW_type = WMC_type / NOM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.AMW_type";
                    AddMetricResult(metricsResults, currentKey, AMW_type);
                }

                processedFiles++;

                progress.Report((double)processedFiles / totalFiles);
            }

            currentNamespace = string.Empty;
            currentProject = project.Name;
            currentKey = string.Empty;

            if (isMethodCodeSmell)
            {
                foreach (var document in project.Documents)
                {
                    var root = await document.GetSyntaxRootAsync();
                    if (root == null) continue;

                    var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
                    var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                    foreach (var classDeclaration in classDeclarations)
                    {
                        currentNamespace = GetNamespace(classDeclaration);
                        var className = classDeclaration.Identifier.ToString();
                        var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
                        currentKeyBase = $"{currentProject}.{currentNamespace}.{className}";

                        foreach (var method in methods)
                        {
                            var nameMethod = $"{method.Identifier}({string.Join(", ", method.ParameterList.Parameters)})";

                            var currentKeyBaseMethod = $"{currentKeyBase}.{nameMethod}";

                            var keyLabeledMethods = $"{currentNamespace}.{className}.{method.Identifier}";

                            if (labeledCodeSmell.ContainsKey(keyLabeledMethods))
                            {
                                string severity = labeledCodeSmell[keyLabeledMethods];
                                string multiclass = severity;
                                string smell = string.Empty;

                                try
                                {
                                    LabeledCodeSmellMetrics labeledCodeSmellMetrics = new LabeledCodeSmellMetrics();
                                    labeledCodeSmellMetrics.CodeSmell = codeSmell;
                                    labeledCodeSmellMetrics.Project = currentProject;
                                    labeledCodeSmellMetrics.Package = currentNamespace;
                                    labeledCodeSmellMetrics.ClassName = className;
                                    labeledCodeSmellMetrics.Method = nameMethod;
                                    labeledCodeSmellMetrics.IsSmell = (!severity.Equals("0"));
                                    labeledCodeSmellMetrics.SeverityLevel = Convert.ToInt32(severity);
                                    labeledCodeSmellMetrics.Severity = Convert.ToInt32(severity);
                                    labeledCodeSmellMetrics.ATFD_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.ATFD_method"]);
                                    labeledCodeSmellMetrics.FDP_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.FDP_method"]);
                                    labeledCodeSmellMetrics.MAXNESTING_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.MAXNESTING_method"]);
                                    labeledCodeSmellMetrics.LOC_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.LOC_method"]);
                                    labeledCodeSmellMetrics.CYCLO_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.CYCLO_method"]);
                                    labeledCodeSmellMetrics.NOLV_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.NOLV_method"]);
                                    labeledCodeSmellMetrics.NOAV_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.NOAV_method"]);
                                    labeledCodeSmellMetrics.FANOUT_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.FANOUT_method"]);
                                    labeledCodeSmellMetrics.CFNAMM_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.CFNAMM_method"]);
                                    labeledCodeSmellMetrics.ATLD_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.ATLD_method"]);
                                    labeledCodeSmellMetrics.CINT_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.CINT_method"]);
                                    labeledCodeSmellMetrics.CDISP_method = Convert.ToDouble(metricsResults[$"{currentKeyBaseMethod}.CDISP_method"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                    labeledCodeSmellMetrics.AMW_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.AMW_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                    labeledCodeSmellMetrics.ATFD_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.ATFD_type"]);
                                    labeledCodeSmellMetrics.CFNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.CFNAMM_type"]);
                                    labeledCodeSmellMetrics.DIT_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.DIT_type"]);
                                    labeledCodeSmellMetrics.FANOUT_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.FANOUT_type"]);
                                    labeledCodeSmellMetrics.LCOM5_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.LCOM5_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                    labeledCodeSmellMetrics.LOCNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.LOCNAMM_type"]);
                                    labeledCodeSmellMetrics.LOC_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.LOC_package"]);
                                    labeledCodeSmellMetrics.LOC_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.LOC_type"]);
                                    labeledCodeSmellMetrics.NIM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NIM_type"]);
                                    labeledCodeSmellMetrics.NOA_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOA_type"]);
                                    labeledCodeSmellMetrics.NOCS_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.NOCS_package"]);
                                    labeledCodeSmellMetrics.NOCS_project = Convert.ToInt32(metricsResults[$"{currentProject}.NOCS_project"]);
                                    labeledCodeSmellMetrics.NOMNAMM_project = Convert.ToInt32(metricsResults[$"{currentProject}.NOMNAMM_project"]);
                                    labeledCodeSmellMetrics.NOMNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOMNAMM_type"]);
                                    labeledCodeSmellMetrics.NOM_project = Convert.ToInt32(metricsResults[$"{currentProject}.NOM_project"]);
                                    labeledCodeSmellMetrics.RFC_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.RFC_type"]);
                                    labeledCodeSmellMetrics.TCC_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.TCC_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                    labeledCodeSmellMetrics.WMCNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.WMCNAMM_type"]);
                                    labeledCodeSmellMetrics.WMC_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.WMC_type"]);
                                    labeledCodeSmellMetrics.WOC_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.WOC_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                                    labeledCodeSmellMetrics.NOM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOM_type"]);
                                    labeledCodeSmellMetrics.AMWNAMM_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.AMWNAMM_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                    labeledCodeSmellMetrics.NOCS_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOCS_type"]);
                                    labeledCodeSmellMetrics.NOM_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.NOM_package"]);
                                    labeledCodeSmellMetrics.NOMNAMM_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.NOMNAMM_package"]);
                                    labeledCodeSmellMetrics.LOCNAMM_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.LOCNAMM_package"]);
                                    labeledCodeSmellMetrics.LOC_project = Convert.ToInt32(metricsResults[$"{currentProject}.LOC_project"]);
                                    labeledCodeSmellMetrics.LOCNAMM_project = Convert.ToInt32(metricsResults[$"{currentProject}.LOCNAMM_project"]);
                                    
                                    //NOVAS MÉTRICAS
                                    labeledCodeSmellMetrics.NOAM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOAM_type"]);
                                    labeledCodeSmellMetrics.NOP_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.NOP_method"]);
                                    labeledCodeSmellMetrics.MaMCL_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.MaMCL_method"]);
                                    labeledCodeSmellMetrics.MeMCL_method = Convert.ToDouble(metricsResults[$"{currentKeyBaseMethod}.MeMCL_method"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                    labeledCodeSmellMetrics.CLNAMM_method = Convert.ToInt32(metricsResults[$"{currentKeyBaseMethod}.CLNAMM_method"]);
                                    labeledCodeSmellMetrics.num_not_final_not_static_attributes = Convert.ToInt32(metricsResults[$"{currentKeyBase}.num_not_final_not_static_attributes"]);
                                    labeledCodeSmellMetrics.number_private_visibility_attributes = Convert.ToInt32(metricsResults[$"{currentKeyBase}.number_private_visibility_attributes"]);

                                    labeledCodeSmellMetricsList.Add(labeledCodeSmellMetrics);
                                }
                                catch
                                {
                                    Console.WriteLine(
                                    $"{codeSmell};{currentProject};{currentNamespace};{className};{nameMethod};{smell};{multiclass};{severity};Error in extracting metrics;");
                                }
                            }
                        }
                    }
                }
            }
            else //isClassCodeSmell
            {
                foreach (var document in project.Documents)
                {
                    var root = await document.GetSyntaxRootAsync();
                    if (root == null) continue;

                    var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
                    var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                    foreach (var classDeclaration in classDeclarations)
                    {
                        currentNamespace = GetNamespace(classDeclaration);

                        var className = classDeclaration.Identifier.ToString();
                        var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
                        currentKeyBase = $"{currentProject}.{currentNamespace}.{className}";
                        var keyLabeledClasses = $"{currentNamespace}.{className}";

                        if (labeledCodeSmell.ContainsKey(keyLabeledClasses))
                        {
                            string severity = labeledCodeSmell[keyLabeledClasses];
                            string multiclass = severity;
                            string smell = string.Empty;

                            try
                            {
                                LabeledCodeSmellMetrics labeledCodeSmellMetrics = new LabeledCodeSmellMetrics();
                                labeledCodeSmellMetrics.CodeSmell = codeSmell;
                                labeledCodeSmellMetrics.Project = currentProject;
                                labeledCodeSmellMetrics.Package = currentNamespace;
                                labeledCodeSmellMetrics.ClassName = className;
                                labeledCodeSmellMetrics.Method = string.Empty;
                                labeledCodeSmellMetrics.IsSmell = (!severity.Equals("0"));
                                labeledCodeSmellMetrics.SeverityLevel = Convert.ToInt32(severity);
                                labeledCodeSmellMetrics.Severity = Convert.ToInt32(severity);
                                labeledCodeSmellMetrics.ATFD_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.ATFD_method"]);
                                labeledCodeSmellMetrics.FDP_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.FDP_method"]);
                                labeledCodeSmellMetrics.MAXNESTING_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.MAXNESTING_method"]);
                                labeledCodeSmellMetrics.LOC_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.LOC_method"]);
                                labeledCodeSmellMetrics.CYCLO_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.CYCLO_method"]);
                                labeledCodeSmellMetrics.NOLV_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOLV_method"]);
                                labeledCodeSmellMetrics.NOAV_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOAV_method"]);
                                labeledCodeSmellMetrics.FANOUT_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.FANOUT_method"]);
                                labeledCodeSmellMetrics.CFNAMM_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.CFNAMM_method"]);
                                labeledCodeSmellMetrics.ATLD_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.ATLD_method"]);
                                labeledCodeSmellMetrics.CINT_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.CINT_method"]);
                                labeledCodeSmellMetrics.CDISP_method = Convert.ToDouble(metricsResults[$"{currentKeyBase}.CDISP_method"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                labeledCodeSmellMetrics.AMW_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.AMW_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                labeledCodeSmellMetrics.ATFD_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.ATFD_type"]);
                                labeledCodeSmellMetrics.CFNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.CFNAMM_type"]);
                                labeledCodeSmellMetrics.DIT_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.DIT_type"]);
                                labeledCodeSmellMetrics.FANOUT_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.FANOUT_type"]);
                                labeledCodeSmellMetrics.LCOM5_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.LCOM5_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                labeledCodeSmellMetrics.LOCNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.LOCNAMM_type"]);
                                labeledCodeSmellMetrics.LOC_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.LOC_package"]);
                                labeledCodeSmellMetrics.LOC_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.LOC_type"]);
                                labeledCodeSmellMetrics.NIM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NIM_type"]);
                                labeledCodeSmellMetrics.NOA_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOA_type"]);
                                labeledCodeSmellMetrics.NOCS_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.NOCS_package"]);
                                labeledCodeSmellMetrics.NOCS_project = Convert.ToInt32(metricsResults[$"{currentProject}.NOCS_project"]);
                                labeledCodeSmellMetrics.NOMNAMM_project = Convert.ToInt32(metricsResults[$"{currentProject}.NOMNAMM_project"]);
                                labeledCodeSmellMetrics.NOMNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOMNAMM_type"]);
                                labeledCodeSmellMetrics.NOM_project = Convert.ToInt32(metricsResults[$"{currentProject}.NOM_project"]);
                                labeledCodeSmellMetrics.RFC_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.RFC_type"]);
                                labeledCodeSmellMetrics.TCC_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.TCC_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                labeledCodeSmellMetrics.WMCNAMM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.WMCNAMM_type"]);
                                labeledCodeSmellMetrics.WMC_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.WMC_type"]);
                                labeledCodeSmellMetrics.WOC_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.WOC_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                                labeledCodeSmellMetrics.NOM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOM_type"]);
                                labeledCodeSmellMetrics.AMWNAMM_type = Convert.ToDouble(metricsResults[$"{currentKeyBase}.AMWNAMM_type"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                labeledCodeSmellMetrics.NOCS_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOCS_type"]);
                                labeledCodeSmellMetrics.NOM_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.NOM_package"]);
                                labeledCodeSmellMetrics.NOMNAMM_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.NOMNAMM_package"]);
                                labeledCodeSmellMetrics.LOCNAMM_package = Convert.ToInt32(metricsResults[$"{currentProject}.{currentNamespace}.LOCNAMM_package"]);
                                labeledCodeSmellMetrics.LOC_project = Convert.ToInt32(metricsResults[$"{currentProject}.LOC_project"]);
                                labeledCodeSmellMetrics.LOCNAMM_project = Convert.ToInt32(metricsResults[$"{currentProject}.LOCNAMM_project"]);
                                
                                //NOVAS METRICAS
                                labeledCodeSmellMetrics.NOAM_type = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOAM_type"]);
                                labeledCodeSmellMetrics.NOP_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.NOP_method"]);
                                labeledCodeSmellMetrics.MaMCL_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.MaMCL_method"]);
                                labeledCodeSmellMetrics.MeMCL_method = Convert.ToDouble(metricsResults[$"{currentKeyBase}.MeMCL_method"], System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                                labeledCodeSmellMetrics.CLNAMM_method = Convert.ToInt32(metricsResults[$"{currentKeyBase}.CLNAMM_method"]);
                                labeledCodeSmellMetrics.num_not_final_not_static_attributes = Convert.ToInt32(metricsResults[$"{currentKeyBase}.num_not_final_not_static_attributes"]);
                                labeledCodeSmellMetrics.number_private_visibility_attributes = Convert.ToInt32(metricsResults[$"{currentKeyBase}.number_private_visibility_attributes"]);

                                labeledCodeSmellMetricsList.Add(labeledCodeSmellMetrics);
                            }
                            catch
                            {
                                Console.WriteLine(
                                $"{codeSmell};{currentProject};{currentNamespace};{className};;{smell};{multiclass};{severity};Error in extracting metrics;");
                            }
                        }
                    }
                }
            }
            
            return labeledCodeSmellMetricsList;
        }
        private static void AddMetricResult(Dictionary<string, double> metricsResults, string currentKey, double metricValue)
        {
            if (metricsResults.ContainsKey(currentKey))
            {
                metricsResults[currentKey] += metricValue;
            }
            else
            {
                metricsResults[currentKey] = metricValue;
            }
        }
        private static string GetNamespace(ClassDeclarationSyntax classDeclaration)
        {
            var namespaceDeclaration = classDeclaration.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            return namespaceDeclaration != null ? namespaceDeclaration.Name.ToString() : string.Empty;
        }

        /*private static bool IsConstructor(MethodDeclarationSyntax method)
        {
            // Check if the method is actually a constructor
            return method.Parent is ConstructorDeclarationSyntax;
        }*/
        private static bool IsConstructor(MethodDeclarationSyntax method)
        {
            return method.Identifier.ValueText == method.Parent?.ChildTokens().FirstOrDefault(t => t.IsKind(SyntaxKind.IdentifierToken)).ValueText;
        }

        private static Dictionary<string, string> LabeledCodeSmell(string codeSmell)
        {
            switch (codeSmell)
            {
                case LC:
                    isMethodCodeSmell = false;
                    return LabeledLargeClass();
                case DC:
                    isMethodCodeSmell = false;
                    return LabeledDataClass();
                case RB:
                    isMethodCodeSmell = false;
                    return LabeledRefusedBequest();
                case LM:
                    isMethodCodeSmell = true;
                    return LabeledLongMethod();
                case FE:
                    isMethodCodeSmell = true;
                    return LabeledFeatureEnvy();
                default:
                    isMethodCodeSmell = true;
                    return LabeledLongMethod();
            }
        }
        private static Dictionary<string, string> BaseLabeledCodeSmell(string pathToCsv)
        {
            var lines = File.ReadLines(pathToCsv);
            return lines.Select(line => line.Split(',')).ToDictionary(data => data[1], data => data[2]);
        }

        private static Dictionary<string, string> LabeledLongMethod()
        {
            //Slivka et al. 2023's Labeled Long Method 
            string pathToCsv = @"C:\Users\XXXXX\source\repos\Project1\BaseDatasets\DataSet_Long Method_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
        private static Dictionary<string, string> LabeledLargeClass()
        {
            //Slivka et al. 2023's Labeled Large Class
            string pathToCsv = @"C:\Users\XXXXX\source\repos\Project1\BaseDatasets\DataSet_Large Class_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
        private static Dictionary<string, string> LabeledFeatureEnvy()
        {
            //S. Prokic et al. Labeled Feature Envy
            string pathToCsv = @"C:\Users\XXXXX\source\repos\Project1\BaseDatasets\DataSet_Feature Envy_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
        private static Dictionary<string, string> LabeledRefusedBequest()
        {
            //S. Prokic et al. Labeled Feature Envy
            string pathToCsv = @"C:\Users\XXXXX\source\repos\Project1\BaseDatasets\DataSet_Refused Bequest_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
        private static Dictionary<string, string> LabeledDataClass()
        {
            //S. Prokic et al. Labeled Feature Envy
            string pathToCsv = @"C:\Users\XXXXX\source\repos\Project1\BaseDatasets\DataSet_Data Class_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
    }
}
