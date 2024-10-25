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
using MetricsExtrator.ClassMetrics;
using MetricsExtrator.MethodMetrics;

namespace MetricsExtrator
{
    class CodeMetricsAnalyzer
    {
        private const string LC = "Large Class";
        private const string DC = "Data Class";
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
        private ClassMetricsCalculators classMetricsCalculators;
        private MethodMetricsCalculators methodMetricsCalculators;
        private PackageMetrics packageMetrics;
        private ProjectMetrics projectMetrics;
        private List<LabeledCodeSmellMetrics> labeledCodeSmellMetricsList;
        private MetricsUtilities metricsUtilities;

        public CodeMetricsAnalyzer()
        {
            classMetricsCalculators = new ClassMetricsCalculators();
            methodMetricsCalculators = new MethodMetricsCalculators();
            packageMetrics = new PackageMetrics();
            projectMetrics = new ProjectMetrics();
            metricsUtilities = new MetricsUtilities();
        }


        public async Task<List<LabeledCodeSmellMetrics>> CalculateMetrics(string projectPath, string codeSmell, IProgress<double> progress)
        {
            
            labeledCodeSmellMetricsList = new List<LabeledCodeSmellMetrics>();

            var labeledCodeSmell = LabeledCodeSmell(codeSmell);
           
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
                    currentNamespace = metricsUtilities.GetNamespace(classDeclaration);

                    var className = classDeclaration.Identifier.ToString();

                    var methods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
                    var classSymbol = semanticModelDocument.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

                    var LOC_type = classMetricsCalculators.CalculateLinesOfCode(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.LOC_type";
                    AddMetricResult(metricsResults, currentKey, LOC_type);

                    packageMetrics.LOC_package = LOC_type;
                    currentKey = $"{currentProject}.{currentNamespace}.LOC_package";
                    AddMetricResult(metricsResults, currentKey, packageMetrics.LOC_package);

                    projectMetrics.LOC_project = LOC_type;
                    currentKey = $"{currentProject}.LOC_project";
                    AddMetricResult(metricsResults, currentKey, projectMetrics.LOC_project);

                    var LOCNAMM_type = classMetricsCalculators.Locnamm_Calculator.CalculateLOCNAMMForClass(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.LOCNAMM_type";
                    AddMetricResult(metricsResults, currentKey, LOCNAMM_type);

                    packageMetrics.LOCNAMM_package = LOCNAMM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.LOCNAMM_package";
                    AddMetricResult(metricsResults, currentKey, packageMetrics.LOCNAMM_package);

                    projectMetrics.LOCNAMM_project = LOCNAMM_type;
                    currentKey = $"{currentProject}.LOCNAMM_project";
                    AddMetricResult(metricsResults, currentKey, projectMetrics.LOCNAMM_project);

                    var NOCS_type = classMetricsCalculators.CountNestedClasses(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOCS_type";
                    AddMetricResult(metricsResults, currentKey, NOCS_type);

                    packageMetrics.NOCS_package = NOCS_type + 1; // Include the class itself
                    currentKey = $"{currentProject}.{currentNamespace}.NOCS_package";
                    AddMetricResult(metricsResults, currentKey, packageMetrics.NOCS_package);

                    projectMetrics.NOCS_project = NOCS_type + 1; // Include the class itself
                    currentKey = $"{currentProject}.NOCS_project";
                    AddMetricResult(metricsResults, currentKey, projectMetrics.NOCS_project);

                    double NOM_type = classMetricsCalculators.CountMethodsClass(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOM_type";
                    AddMetricResult(metricsResults, currentKey, NOM_type);

                    packageMetrics.NOM_package = NOM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.NOM_package";
                    AddMetricResult(metricsResults, currentKey, packageMetrics.NOM_package);

                    projectMetrics.NOM_project = NOM_type;
                    currentKey = $"{currentProject}.NOM_project";
                    AddMetricResult(metricsResults, currentKey, projectMetrics.NOM_project);

                    var DIT_type = classMetricsCalculators.Dit_Calculator.CalculateClassDIT(classSymbol, project);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.DIT_type";
                    AddMetricResult(metricsResults, currentKey, DIT_type);

                    var NOA_type = classMetricsCalculators.CalculateClassNOA(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOA_type";
                    AddMetricResult(metricsResults, currentKey, NOA_type);

                    var NIM_type = classMetricsCalculators.CalculateNumberOfInheritedMethods(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NIM_type";
                    AddMetricResult(metricsResults, currentKey, NIM_type);

                    var WOC_type = classMetricsCalculators.CalculateWeightOfClass(classDeclaration);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.WOC_type";
                    AddMetricResult(metricsResults, currentKey, WOC_type);

                    var RFC_type = classMetricsCalculators.Rfc_Calculator.CalculateRFCForClass(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.RFC_type";
                    AddMetricResult(metricsResults, currentKey, RFC_type);

                    var LCOM5_type = classMetricsCalculators.Lcom5_Calculator.CalculateLCOM5ForClass(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.LCOM5_type";
                    AddMetricResult(metricsResults, currentKey, LCOM5_type);

                    var TCC_type = classMetricsCalculators.Tcc_Calculator.CalculateTCCForClass(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.TCC_type";
                    AddMetricResult(metricsResults, currentKey, TCC_type);

                    var num_not_final_not_static_attributes = classMetricsCalculators.CalculateNonFinalNonStaticAttributes(classDeclaration, semanticModelDocument);
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.num_not_final_not_static_attributes";
                    AddMetricResult(metricsResults, currentKey, num_not_final_not_static_attributes);

                    var number_private_visibility_attributes = classMetricsCalculators.CalculateNumberOfPrivateAttributes(classDeclaration);
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
                    classMetricsCalculators.Wmc_type = 0;
                    classMetricsCalculators.Wmcnamm_type = 0;
                    classMetricsCalculators.Amw_type = 0;
                    classMetricsCalculators.Amwnamm_type = 0;
                    classMetricsCalculators.Nomnamm_type = 0;
                    classMetricsCalculators.Fanout_type = 0;
                    classMetricsCalculators.Cfnamm_type = 0;
                    classMetricsCalculators.Noam_type = 0;
                    classMetricsCalculators.Atfd_type = classMetricsCalculators.CalculateATFDForClass(classDeclaration, semanticModelCompilation);

                    foreach (var method in methods)
                    {
                        var nameMethod = $"{method.Identifier}({string.Join(", ", method.ParameterList.Parameters)})";

                        double CYCLO_method = methodMetricsCalculators.CalculateCyclomaticComplexity(method);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CYCLO_method";
                        AddMetricResult(metricsResults, currentKey, CYCLO_method);
                        classMetricsCalculators.Wmc_type += CYCLO_method;

                        if (metricsUtilities.IsAccessorOrMutator(method))
                        {
                            classMetricsCalculators.Noam_type++;
                        }
                        else
                        {
                            classMetricsCalculators.Wmcnamm_type += CYCLO_method;
                        }

                        var NOP_method = methodMetricsCalculators.CalculateNOPForMethod(method);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.NOP_method";
                        AddMetricResult(metricsResults, currentKey, NOP_method);

                        var MaMCL_method = methodMetricsCalculators.Mamcl_Calculator.CalculateMaMCLForMethod(method);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.MaMCL_method";
                        AddMetricResult(metricsResults, currentKey, MaMCL_method);

                        var MeMCL_method = methodMetricsCalculators.Memcl_Calculator.CalculateMeMCLForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.MeMCL_method";
                        AddMetricResult(metricsResults, currentKey, MeMCL_method);

                        var CLNAMM_method = methodMetricsCalculators.Clnamm_Calculator.CalculateCLNAMMForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CLNAMM_method";
                        AddMetricResult(metricsResults, currentKey, CLNAMM_method);

                        var LOC_method = methodMetricsCalculators.CalculateLinesOfCode(method);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.LOC_method";
                        AddMetricResult(metricsResults, currentKey, LOC_method);

                        var ATFD_method = methodMetricsCalculators.Atfd_Calculator.CalculateATFDForMethod(method, semanticModelCompilation);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.ATFD_method";
                        AddMetricResult(metricsResults, currentKey, ATFD_method);
                        classMetricsCalculators.Atfd_type += ATFD_method;

                        var accessesMember = metricsUtilities.IsAccessesMember(method, semanticModelDocument);
                        if (!accessesMember)
                        {
                            classMetricsCalculators.Nomnamm_type++;
                        }

                        var invokedMethods = methodMetricsCalculators.Cint_Calculator.CalculateCINTForMethod(method, semanticModelDocument, classDeclaration);

                        double CINT_method = invokedMethods.Count();
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CINT_method";
                        AddMetricResult(metricsResults, currentKey, CINT_method);

                        double FANOUT_method = methodMetricsCalculators.Fanout_Calculator.CalculateFANOUTForMethod(invokedMethods);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.FANOUT_method";
                        AddMetricResult(metricsResults, currentKey, FANOUT_method);
                        classMetricsCalculators.Fanout_type += FANOUT_method;

                        methodMetricsCalculators.Cdisp_method = 0;
                        if (CINT_method != 0)
                            methodMetricsCalculators.Cdisp_method = FANOUT_method / CINT_method;
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CDISP_method";
                        AddMetricResult(metricsResults, currentKey, methodMetricsCalculators.Cdisp_method);

                        var ATLD_method = methodMetricsCalculators.Atld_Calculator.CalculateMethodATLD(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.ATLD_method";
                        AddMetricResult(metricsResults, currentKey, ATLD_method);

                        var CFNAMM_method = methodMetricsCalculators.Cfnamm_Calculator.CalculateMethodCFNAMM(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.CFNAMM_method";
                        AddMetricResult(metricsResults, currentKey, CFNAMM_method);

                        if (!metricsUtilities.IsConstructor(method) && !method.Modifiers.Any(SyntaxKind.AbstractKeyword))
                        {
                            classMetricsCalculators.Cfnamm_type += CFNAMM_method;
                        }

                        var MAXNESTING_method = methodMetricsCalculators.Maxnesting_Calculator.GetMaxNestingLevel(method.Body);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.MAXNESTING_method";
                        AddMetricResult(metricsResults, currentKey, MAXNESTING_method);

                        var FDP_method = methodMetricsCalculators.Fdp_Calculator.CalculateFDPForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.FDP_method";
                        AddMetricResult(metricsResults, currentKey, FDP_method);

                        var NOAV_method = methodMetricsCalculators.Noav_Calculator.CalculateNOAVForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.NOAV_method";
                        AddMetricResult(metricsResults, currentKey, NOAV_method);

                        var NOLV_method = methodMetricsCalculators.Nolv_Calculator.CalculateNOLVForMethod(method, semanticModelDocument);
                        currentKey = $"{currentProject}.{currentNamespace}.{className}.{nameMethod}.NOLV_method";
                        AddMetricResult(metricsResults, currentKey, NOLV_method);
                    }

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.FANOUT_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Fanout_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.CFNAMM_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Cfnamm_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOAM_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Noam_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.WMCNAMM_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Wmcnamm_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.WMC_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Wmc_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.ATFD_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Atfd_type);

                    currentKey = $"{currentProject}.{currentNamespace}.{className}.NOMNAMM_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Nomnamm_type);

                    double NOMNAMM_package = classMetricsCalculators.Nomnamm_type;
                    currentKey = $"{currentProject}.{currentNamespace}.NOMNAMM_package";
                    AddMetricResult(metricsResults, currentKey, NOMNAMM_package);

                    double NOMNAMM_project = classMetricsCalculators.Nomnamm_type;
                    currentKey = $"{currentProject}.NOMNAMM_project";
                    AddMetricResult(metricsResults, currentKey, NOMNAMM_project);

                    if (classMetricsCalculators.Nomnamm_type != 0)
                        classMetricsCalculators.Amwnamm_type = classMetricsCalculators.Wmcnamm_type / classMetricsCalculators.Nomnamm_type;
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.AMWNAMM_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Amwnamm_type);


                    if (NOM_type != 0)
                        classMetricsCalculators.Amw_type = classMetricsCalculators.Wmc_type / NOM_type;
                    currentKey = $"{currentProject}.{currentNamespace}.{className}.AMW_type";
                    AddMetricResult(metricsResults, currentKey, classMetricsCalculators.Amw_type);
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
                        currentNamespace = metricsUtilities.GetNamespace(classDeclaration);
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
                        currentNamespace = metricsUtilities.GetNamespace(classDeclaration);

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
            string pathToCsv = @"C:\Users\XXXXXXX\source\repos\MetricsAnalyzer\BaseDatasets\DataSet_Long Method_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
        private static Dictionary<string, string> LabeledLargeClass()
        {
            //Slivka et al. 2023's Labeled Large Class
            string pathToCsv = @"C:\Users\XXXXXXX\source\repos\MetricsAnalyzer\BaseDatasets\DataSet_Large Class_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
        private static Dictionary<string, string> LabeledFeatureEnvy()
        {
            //S. Prokic et al. Labeled Feature Envy
            string pathToCsv = @"C:\Users\XXXXXXX\source\repos\MetricsAnalyzer\BaseDatasets\DataSet_Feature Envy_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
        private static Dictionary<string, string> LabeledDataClass()
        {
            //S. Prokic et al. Labeled Feature Envy
            string pathToCsv = @"C:\Users\XXXXXXX\source\repos\MetricsAnalyzer\BaseDatasets\DataSet_Data Class_All projects.csv";
            return BaseLabeledCodeSmell(pathToCsv);
        }
    }
}
